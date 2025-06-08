using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using SBM.Shared.Entities;

namespace SBM.WEB.Pages.Categories
{
    public partial class CategoryForm
    {
        private EditContext editContext = null!;

        [Parameter]
        [EditorRequired]
        public Category Category { get; set; } = null!;

        [Parameter]
        [EditorRequired]
        public EventCallback OnValidSubmit { get; set; }

        [Parameter]
        [EditorRequired]
        public EventCallback ReturnAction { get; set; }

        public bool FormPostedSuccessfully { get; set; }

        protected override void OnInitialized()
        {
            editContext = new(Category);
        }

        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            var formWasMofied = editContext.IsModified();
            if (!formWasMofied || FormPostedSuccessfully)
            {
                return;
            }

            var result = await sweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Deseas abandonar la página y perder los cambios?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
                CancelButtonText = "No",
                ConfirmButtonText = "Si"
            });

            var confirm = !string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            context.PreventNavigation();
        }
    }
}