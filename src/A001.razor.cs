using MetaFrm.Database;
using MetaFrm.Extensions;
using MetaFrm.Management.Razor.Models;
using MetaFrm.Management.Razor.ViewModels;
using MetaFrm.Razor.Essentials;
using MetaFrm.Service;
using MetaFrm.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Reflection;

namespace MetaFrm.Management.Razor
{
    /// <summary>
    /// A001
    /// </summary>
    public partial class A001
    {
        #region Variable
        internal A001ViewModel A001ViewModel { get; set; } = Factory.CreateViewModel<A001ViewModel>();

        internal DataGridControl<AssemblyModel>? DataGridControl;

        internal DataGridControl<AttributeModel>? DataGridControlAttribute;

        internal AssemblyModel SelectItem = new();
        #endregion


        #region Init
        /// <summary>
        /// OnAfterRenderAsync
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (!this.IsLogin())
                    this.Navigation?.NavigateTo("/", true);

                this.A001ViewModel = await this.GetSession<A001ViewModel>(nameof(this.A001ViewModel));

                this.Search();

                this.StateHasChanged();
            }
        }
        #endregion


        #region IO
        private void New()
        {
            this.SelectItem = new();
        }

        private void OnSearch()
        {
            if (this.DataGridControl != null)
                this.DataGridControl.CurrentPageNumber = 1;

            this.Search();
        }
        private void Search()
        {
            Response response;

            try
            {
                if (this.A001ViewModel.IsBusy) return;

                this.A001ViewModel.IsBusy = true;

                ServiceData serviceData = new()
                {
                    Token = this.UserClaim("Token")
                };
                serviceData["1"].CommandText = this.GetAttribute("Search");
                serviceData["1"].AddParameter(nameof(this.A001ViewModel.SearchModel.SEARCH_TEXT), DbType.NVarChar, 4000, this.A001ViewModel.SearchModel.SEARCH_TEXT);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.UserClaim("Account.USER_ID").ToInt());
                serviceData["1"].AddParameter("PAGE_NO", DbType.Int, 3, this.DataGridControl != null ? this.DataGridControl.CurrentPageNumber : 1);
                serviceData["1"].AddParameter("PAGE_SIZE", DbType.Int, 3, this.DataGridControl != null && this.DataGridControl.PagingEnabled ? this.DataGridControl.PagingSize : int.MaxValue);

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    this.A001ViewModel.SelectResultModel.Clear();

                    if (response.DataSet != null && response.DataSet.DataTables.Count > 0)
                    {
                        var orderResult = response.DataSet.DataTables[0].DataRows.OrderBy(x => x.String(nameof(Models.AssemblyModel.NAMESPACE)));

                        foreach (var datarow in orderResult)
                        {
                            this.A001ViewModel.SelectResultModel.Add(new AssemblyModel
                            {
                                ASSEMBLY_ID = datarow.Int(nameof(AssemblyModel.ASSEMBLY_ID)),
                                NAMESPACE = datarow.String(nameof(AssemblyModel.NAMESPACE)),
                                FILE_DATE = datarow.DateTime(nameof(AssemblyModel.FILE_DATE)),
                                VERSION = datarow.String(nameof(AssemblyModel.VERSION)),
                                PLATFORM_ID = datarow.Int(nameof(AssemblyModel.PLATFORM_ID)),
                                PLATFORM_DESC = datarow.String(nameof(AssemblyModel.PLATFORM_DESC)),
                                NICKNAME = datarow.String(nameof(AssemblyModel.NICKNAME)),
                            });
                        }

                        //this.DataGridControl?.SortInit(this.ColumnDefinitions, nameof(SelectResultModel.NAMESPACE), SortDirection.Ascending);
                        this.DataGridControl?.SortData();
                        //this.DataGridControl.Pages = new int[] { 1, 2, 3, 4 };
                    }
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.A001ViewModel.IsBusy = false;
                this.SetSession(nameof(A001ViewModel), this.A001ViewModel);
            }
        }
        private void SearchAttribute()
        {
            Response response;

            try
            {
                if (this.SelectItem == null || this.SelectItem.ASSEMBLY_ID == null) return;
                if (this.A001ViewModel.IsBusy) return;

                this.A001ViewModel.IsBusy = true;

                ServiceData serviceData = new()
                {
                    Token = this.UserClaim("Token")
                };
                serviceData["1"].CommandText = this.GetAttribute("SearchAttribute");
                serviceData["1"].AddParameter(nameof(this.SelectItem.ASSEMBLY_ID), DbType.Int, 3, this.SelectItem.ASSEMBLY_ID);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.UserClaim("Account.USER_ID").ToInt());
                serviceData["1"].AddParameter("PAGE_NO", DbType.Int, 3, 1);
                serviceData["1"].AddParameter("PAGE_SIZE", DbType.Int, 3, int.MaxValue);

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    this.SelectItem.Attributes.Clear();

                    if (response.DataSet != null && response.DataSet.DataTables.Count > 0)
                    {
                        var orderResult = response.DataSet.DataTables[0].DataRows.OrderBy(x => x.String(nameof(AttributeModel.ATTRIBUTE_NAME)));

                        foreach (var datarow in orderResult)
                        {
                            this.SelectItem.Attributes.Add(new AttributeModel
                            {
                                ATTRIBUTE_ID = datarow.Int(nameof(AttributeModel.ATTRIBUTE_ID)),
                                ASSEMBLY_ID = datarow.Int(nameof(AttributeModel.ASSEMBLY_ID)),
                                ATTRIBUTE_NAME = datarow.String(nameof(AttributeModel.ATTRIBUTE_NAME)),
                                ATTRIBUTE_VALUE = datarow.String(nameof(AttributeModel.ATTRIBUTE_VALUE)),
                            });
                        }
                    }
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.A001ViewModel.IsBusy = false;
                this.SetSession(nameof(A001ViewModel), this.A001ViewModel);
            }
        }

        private async void Save()
        {
            Response? response;
            string? value;
            byte[] bytes;

            response = null;

            try
            {
                if (this.A001ViewModel.IsBusy)
                    return;

                if (this.SelectItem == null)
                    return;

                this.A001ViewModel.IsBusy = true;

                ServiceData serviceData = new()
                {
                    TransactionScope = true,
                    Token = this.UserClaim("Token")
                };
                serviceData["1"].CommandText = this.GetAttribute("Save");
                serviceData["1"].AddParameter(nameof(this.SelectItem.ASSEMBLY_ID), DbType.Int, 3, "2", nameof(this.SelectItem.ASSEMBLY_ID), this.SelectItem.ASSEMBLY_ID);
                serviceData["1"].AddParameter(nameof(this.SelectItem.NAMESPACE), DbType.NVarChar, 4000, this.SelectItem.NAMESPACE);

                Assembly? assembly = null;

                if (this.SelectItem.DllFile == null)
                    serviceData["1"].AddParameter("FILE_TEXT", DbType.Text, 0, null);
                else
                {
                    bytes = new byte[this.SelectItem.DllFile.Size];
                    //int count = await this.SelectItem.DllFile.OpenReadStream(10000 * 1024).ReadAsync(bytes.AsMemory(0, (int)this.SelectItem.DllFile.Size));

                    MemoryStream memoryStream = new();
                    await this.SelectItem.DllFile.OpenReadStream(10000 * 1024).CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    int count = await memoryStream.ReadAsync(bytes.AsMemory(0, (int)this.SelectItem.DllFile.Size));

                    assembly = Assembly.Load(bytes);

                    if (count > 0)
                        serviceData["1"].AddParameter("FILE_TEXT", DbType.Text, 0, Convert.ToBase64String(bytes));
                }

                if (assembly != null)
                {

                    if (assembly.GetName().Version != null)
                    {
                        serviceData["1"].AddParameter(nameof(this.SelectItem.FILE_DATE), DbType.DateTime, 0, assembly.GetLinkerTime());
                        serviceData["1"].AddParameter(nameof(this.SelectItem.VERSION), DbType.NVarChar, 25, assembly.GetName().Version?.ToString());
                    }
                    else
                    {
                        serviceData["1"].AddParameter(nameof(this.SelectItem.FILE_DATE), DbType.DateTime, 0, this.SelectItem.FILE_DATE);
                        serviceData["1"].AddParameter(nameof(this.SelectItem.VERSION), DbType.NVarChar, 25, this.SelectItem.VERSION);
                    }
                }
                else
                    serviceData["1"].AddParameter(nameof(this.SelectItem.VERSION), DbType.NVarChar, 25, this.SelectItem.VERSION);

                serviceData["1"].AddParameter(nameof(this.SelectItem.PLATFORM_ID), DbType.Int, 3, Factory.ProjectService.PlatformID);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.UserClaim("Account.USER_ID").ToInt());

                if (this.SelectItem.Attributes != null && this.SelectItem.Attributes.Count > 0)
                {
                    serviceData["2"].CommandText = this.GetAttribute("SaveAttribute");
                    serviceData["2"].AddParameter(nameof(AttributeModel.ATTRIBUTE_ID), DbType.Int, 3);
                    serviceData["2"].AddParameter(nameof(AttributeModel.ASSEMBLY_ID), DbType.Int, 3);
                    serviceData["2"].AddParameter(nameof(AttributeModel.ATTRIBUTE_NAME), DbType.NVarChar, 4000);
                    serviceData["2"].AddParameter(nameof(AttributeModel.ATTRIBUTE_VALUE), DbType.NVarChar, 4000);
                    serviceData["2"].AddParameter("USER_ID", DbType.Int, 3);

                    foreach (var item in this.SelectItem.Attributes)
                    {
                        serviceData["2"].NewRow();
                        serviceData["2"].SetValue(nameof(AttributeModel.ATTRIBUTE_ID), item.ATTRIBUTE_ID);
                        serviceData["2"].SetValue(nameof(AttributeModel.ASSEMBLY_ID), item.ASSEMBLY_ID);
                        serviceData["2"].SetValue(nameof(AttributeModel.ATTRIBUTE_NAME), item.ATTRIBUTE_NAME);
                        serviceData["2"].SetValue(nameof(AttributeModel.ATTRIBUTE_VALUE), item.ATTRIBUTE_VALUE);
                        serviceData["2"].SetValue("USER_ID", this.UserClaim("Account.USER_ID").ToInt());
                    }
                }

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    if (response.DataSet != null && response.DataSet.DataTables.Count > 0 && response.DataSet.DataTables[0].DataRows.Count > 0 && this.SelectItem != null && this.SelectItem.ASSEMBLY_ID == null)
                    {
                        value = response.DataSet.DataTables[0].DataRows[0].String("Value");

                        if (value != null)
                            this.SelectItem.ASSEMBLY_ID = value.ToInt();
                    }

                    this.ToastShow("Completed", $"{this.GetAttribute("Title")} registered successfully.", Alert.ToastDuration.Long);
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.A001ViewModel.IsBusy = false;

                if (response != null && response.Status == Status.OK)
                {
                    this.Search();
                    this.SearchAttribute();
                    this.StateHasChanged();
                }
            }
        }

        private void Delete()
        {
            this.ModalShow($"Question", "Do you want to delete?", new() { { "Delete", Btn.Danger }, { "Cancel", Btn.Primary } }, EventCallback.Factory.Create<string>(this, DeleteAction));
        }
        private void DeleteAction(string action)
        {
            Response? response;

            response = null;

            try
            {
                if (action != "Delete") return;

                if (this.A001ViewModel.IsBusy) return;

                if (this.SelectItem == null) return;

                this.A001ViewModel.IsBusy = true;

                if (this.SelectItem.ASSEMBLY_ID == null || this.SelectItem.ASSEMBLY_ID <= 0)
                {
                    this.ToastShow("Warning", $"Please select an {this.GetAttribute("Title")} to delete.", Alert.ToastDuration.Long);
                    return;
                }

                ServiceData serviceData = new()
                {
                    TransactionScope = true,
                    Token = this.UserClaim("Token")
                };
                serviceData["1"].CommandText = this.GetAttribute("Delete");
                serviceData["1"].AddParameter(nameof(this.SelectItem.ASSEMBLY_ID), DbType.Int, 3, this.SelectItem.ASSEMBLY_ID);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.UserClaim("Account.USER_ID").ToInt());

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    this.New();
                    this.ToastShow("Completed", $"{this.GetAttribute("Title")} deleted successfully.", Alert.ToastDuration.Long);
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.A001ViewModel.IsBusy = false;

                if (response != null && response.Status == Status.OK)
                {
                    this.Search();
                    this.StateHasChanged();
                }
            }
        }

        private void DeleteAttribute(AttributeModel? attributeModel)
        {
            Response? response;

            try
            {
                if (this.A001ViewModel.IsBusy)
                    return;

                if (attributeModel == null)
                    return;

                this.A001ViewModel.IsBusy = true;

                if (attributeModel.ATTRIBUTE_ID == null || attributeModel.ATTRIBUTE_ID <= 0)
                {
                    this.SelectItem.Attributes.Remove(attributeModel);
                    this.ToastShow("Completed", "Attribute deleted successfully.", Alert.ToastDuration.Long);
                    return;
                }

                ServiceData serviceData = new()
                {
                    TransactionScope = true,
                    Token = this.UserClaim("Token")
                };
                serviceData["1"].CommandText = this.GetAttribute("DeleteAttribute");
                serviceData["1"].AddParameter(nameof(attributeModel.ATTRIBUTE_ID), DbType.Int, 3, attributeModel.ATTRIBUTE_ID);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.UserClaim("Account.USER_ID").ToInt());

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    this.SelectItem.Attributes.Remove(attributeModel);
                    this.ToastShow("Completed", "Attribute deleted successfully.", Alert.ToastDuration.Long);
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.A001ViewModel.IsBusy = false;
            }
        }
        #endregion


        #region Event
        private void SearchKeydown(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                this.OnSearch();
            }
        }

        private void InputFileChangeEventArgs(Microsoft.AspNetCore.Components.Forms.InputFileChangeEventArgs e)
        {
            if (this.SelectItem != null && e.FileCount == 1)
                this.SelectItem.DllFile = e.File;
        }

        private void RowModify(AssemblyModel item)
        {
            this.SelectItem = new()
            {
                ASSEMBLY_ID = item.ASSEMBLY_ID,
                NAMESPACE = item.NAMESPACE,
                FILE_DATE = item.FILE_DATE,
                VERSION = item.VERSION,
                PLATFORM_ID = item.PLATFORM_ID,
                PLATFORM_DESC = item.PLATFORM_DESC,
                NICKNAME = item.NICKNAME,
            };

            this.SearchAttribute();
        }

        private void AddAttribute()
        {
            this.SelectItem.Attributes.Add(new AttributeModel { ASSEMBLY_ID = this.SelectItem.ASSEMBLY_ID });
        }

        private void Copy()
        {
            if (this.SelectItem != null)
            {
                this.SelectItem.ASSEMBLY_ID = null;
                this.SelectItem.DllFile = null;
                this.SelectItem.FILE_DATE = null;
                this.SelectItem.VERSION = null;
                this.SelectItem.NICKNAME = null;
            }

            if (this.SelectItem != null && this.SelectItem.Attributes != null)
                foreach (var item in this.SelectItem.Attributes)
                {
                    item.ATTRIBUTE_ID = null;
                    item.ASSEMBLY_ID = null;
                }
        }
        #endregion
    }
}