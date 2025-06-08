using Blazored.Modal;
using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using SBM.Shared.Entities;
using System.Net;

namespace SBM.WEB.Pages.Categories
{
    public partial class CategoryEdit
    {
        private Category? category;
        private CategoryForm? categoryForm;

        [CascadingParameter]
        BlazoredModalInstance BlazoredModal { get; set; } = default!;

        [Parameter]
        public int CategoryId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var responseHttp = await repository.Get<Category>($"/api/categories/{CategoryId}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    navigationManager.NavigateTo("/categories");
                    return;
                }

                var message = await responseHttp.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            category = responseHttp.Response?.Result;
        }

        private async Task EditAsync()
        {
            var responseHttp = await repository.Put("/api/categories", category);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            await BlazoredModal.CloseAsync(ModalResult.Ok());
            Return();
        }

        private void Return()
        {
            categoryForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo($"/categories");
        }
    }
}