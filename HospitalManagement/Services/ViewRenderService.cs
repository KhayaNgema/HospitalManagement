using HospitalManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HospitalManagement.Services
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public ViewRenderService(
            ICompositeViewEngine viewEngine,
            IHttpContextAccessor contextAccessor,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _contextAccessor = contextAccessor;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderToStringAsync(string viewName, object model)
        {
            var httpContext = _contextAccessor.HttpContext;

            var actionContext = new ActionContext(httpContext,
                httpContext.GetRouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

            using var sw = new StringWriter();


            var viewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: false);
            if (!viewResult.Success)
            {
                var searchedLocations = string.Join(Environment.NewLine, viewResult.SearchedLocations);
                throw new InvalidOperationException($"Couldn't find view '{viewName}'. Searched in:{Environment.NewLine}{searchedLocations}");
            }


            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            var tempData = new TempDataDictionary(httpContext, _tempDataProvider);

            var viewContext = new ViewContext(actionContext, viewResult.View, viewDictionary, tempData, sw, new HtmlHelperOptions());

            await viewResult.View.RenderAsync(viewContext);

            return sw.ToString();
        }
    }
}
