using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using SBM.Shared.Entities;
using SBM.Shared.Responses;

namespace SBM.WEB.Pages.Categories
{
    public partial class CategoriesIndex
    {
        public List<Category>? categories { get; set; }
        private int currentPage = 1;
        private int totalPages;

        [CascadingParameter]
        IModalService Modal { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public string Page { get; set; } = "";

        [Parameter]
        [SupplyParameterFromQuery]
        public string Filter { get; set; } = "";

        protected async override Task OnInitializedAsync()
        {
            await LoadAsync();
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
                url1 = $"api/categories?page={page}";
                url2 = $"api/categories/totalPages";
            }
            else
            {
                url1 = $"api/categories?page={page}&filter={Filter}";
                url2 = $"api/categories/totalPages?filter={Filter}";
            }

            try
            {
                var responseHppt = await repository.Get<List<Category>>(url1);
                //var responseHppt2 = await repository.Get<int>(url2);
                var responseHppt2 = await repository.Get<Response<object>>(url2);
                categories = responseHppt.Response?.Result!;
                totalPages = Convert.ToInt32(responseHppt2.Response?.TotalPages);
            }
            catch (Exception ex)
            {
                await sweetAlertService.FireAsync("Error", ex.Message, SweetAlertIcon.Error);
            }
        }

        private async Task Delete(int categoryId)
        {
            var result = await sweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Esta seguro que quieres borrar el registro?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true
            });

            var confirm = string.IsNullOrEmpty(result.Value);

            if (confirm)
            {
                return;
            }

            var responseHTTP = await repository.Delete($"api/categories/{categoryId}");

            if (responseHTTP.Error)
            {
                if (responseHTTP.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    navigationManager.NavigateTo("/");
                }
                else
                {
                    var mensajeError = await responseHTTP.GetErrorMessageAsync();
                    await sweetAlertService.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
                }
            }
            else
            {
                await LoadAsync();
            }
        }
    }
}