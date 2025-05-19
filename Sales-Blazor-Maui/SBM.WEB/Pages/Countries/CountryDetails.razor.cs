using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using SBM.Shared.Entities;
using System.Net;

namespace SBM.WEB.Pages.Countries
{
    public partial class CountryDetails
    {
        private Country? country;
        //private List<State>? states;
        private int currentPage = 1;
        private int totalPages;

        [Parameter]
        public int Id { get; set; }


        [Parameter]
        [SupplyParameterFromQuery]
        public string Page { get; set; } = "";

        [Parameter]
        [SupplyParameterFromQuery]
        public string Filter { get; set; } = "";

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task SelectedPageAsync(int page)
        {
            currentPage = page;
            await LoadAsync(page);
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
                url1 = $"api/states?id={Id}&page={page}";
                url2 = $"api/states/totalPages?id={Id}";
            }
            else
            {
                url1 = $"api/states?id={Id}&page={page}&filter={Filter}";
                url2 = $"api/states/totalPages?id={Id}&filter={Filter}";
            }

            var responseHppt1 = await repository.Get<Country>($"api/countries/{Id}");
            //var responseHppt2 = await repository.Get<List<State>>(url1);
            //var responseHppt3 = await repository.Get<int>(url2);
            country = responseHppt1.Response?.Result;
            //states = responseHppt2.Response;
            //totalPages = responseHppt3.Response;
        }

        private async Task DeleteAsync(int id)
        {
            var result = await sweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Realmente deseas eliminar el registro?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
                CancelButtonText = "No",
                ConfirmButtonText = "Si"
            });

            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHttp = await repository.Delete($"/api/states/{id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode != HttpStatusCode.NotFound)
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                    return;
                }
            }

            await LoadAsync();
        }

        private async Task CleanFilterAsync()
        {
            Filter = string.Empty;
            await ApplyFilterAsync();
        }

        private async Task ApplyFilterAsync()
        {
            int page = 1;
            await LoadAsync(page);
            await SelectedPageAsync(page);
        }
    }
}