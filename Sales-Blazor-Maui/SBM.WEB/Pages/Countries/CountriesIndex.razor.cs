using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using SBM.Shared.Entities;

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
                Countries = responseHppt.Response.Result!;
                //totalPages = responseHppt2.Response!;
            }
            catch (Exception ex)
            {
                await sweetAlertService.FireAsync("Error", ex.Message, SweetAlertIcon.Error);
            }
        }
    }
}