using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using SBM.Shared.Entities;
using SBM.Shared.Responses;
using SBM.WEB.Repositories;


namespace SBM.WEB.Pages.Countries
{
    public partial class CountryCreate
    {
        private Country country = new();
        private CountryForm? countryForm;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;

        private async Task CreateAsync()
        {
            var httpResponse = await Repository.Post<Country, Country>("/api/countries", country);
            if (!httpResponse.Error && httpResponse.Response?.IsSuccess == false)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            Return();
        }

        private void Return()
        {
            countryForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo("/countries");
        }
    }
}