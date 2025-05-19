using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SBM.Shared.Entities;
using SBM.WEB.Repositories;
using System.Net;

namespace SBM.WEB.Pages.Countries
{
    public partial class CountriesIndex
    {
        private int currentPage = 1;
        private int totalPages;
        public List<Country>? Countries { get; set; }
        [Parameter]
        [SupplyParameterFromQuery]
        public string Page { get; set; } = string.Empty;

        [Parameter]
        [SupplyParameterFromQuery]
        public string Filter { get; set; } = string.Empty;

        private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/countries";

        private string infoFormat = "{first_item}-{last_item} => {all_items}";

        //[Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            var responseHttp = await repository.Get<List<Country>>("/api/countries/full");

            if (!responseHttp.Error && responseHttp.Response?.IsSuccess == true)
            {
                Countries = responseHttp.Response.Result;
            }
            else
            {
                var errorMessage = await responseHttp.GetErrorMessageAsync();
                // mostrar en UI
            }
        }

        private async Task LoadAsync(int page = 1)
        {
            if (!string.IsNullOrWhiteSpace(Page))
            {
                page = Convert.ToInt32(Page);
            }

            string url1 = string.Empty;
            string url2 = string.Empty;

            if (string.IsNullOrEmpty(Filter))
            {
                url1 = $"api/countries?page={page}";
                url2 = $"api/countries/totalPages";
            }
            else
            {
                url1 = $"api/countries?page={page}&filter={Filter}";
                url2 = $"api/countries/totalPages?filter={Filter}";
            }

            try
            {
                var responseHppt = await repository.Get<List<Country>>(url1);
                var responseHppt2 = await repository.Get<int>(url2);
                Countries = responseHppt.Response?.Result;
                //totalPages = responseHppt2.Response!;
            }
            catch (Exception ex)
            {
                await sweetAlertService.FireAsync("Error", ex.Message, SweetAlertIcon.Error);
            }
        }

        private async Task DeleteAsync(Country country)
        {
            //var parameters = new DialogParameters
            //{
            //    { "Message", string.Format(Localizer["DeleteConfirm"], Localizer["Country"], country.Name) }
            //};
            //var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            //var dialog = DialogService.Show<ConfirmDialog>(Localizer["Confirmation"], parameters, options);
            //var result = await dialog.Result;
            //if (result!.Canceled)
            //{
            //    return;
            //}

            var responseHttp = await Repository.Delete($"{baseUrl}/{country.Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/countries");
                }
                else
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    //Snackbar.Add(Localizer[message!], Severity.Error);
                }
                return;
            }
            //await LoadTotalRecordsAsync();
            //await table.ReloadServerData();
            //Snackbar.Add(Localizer["RecordDeletedOk"], Severity.Success);
        }
    }
}