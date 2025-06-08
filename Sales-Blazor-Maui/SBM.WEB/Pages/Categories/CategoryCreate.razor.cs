using Blazored.Modal;
using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using SBM.Shared.Entities;

namespace SBM.WEB.Pages.Categories
{
    public partial class CategoryCreate
    {
        private Category category = new();
        private CategoryForm? categoryForm;

        [CascadingParameter]
        BlazoredModalInstance BlazoredModal { get; set; } = default!;

        [Parameter]
        public int StateId { get; set; }

        private async Task CreateAsync()
        {
            var httpResponse = await repository.Post("/api/categories", category);
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
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