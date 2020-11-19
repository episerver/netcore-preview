using System;
using System.IO;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Security;

using AlloyTemplates.Models.ViewModels;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace AlloyTemplates.Business.Rendering
{
    /// <summary>
    /// Wraps an MvcContentRenderer and adds error handling to ensure that blocks and other content
    /// rendered as parts of pages won't crash the entire page if a non-critical exception occurs while rendering it.
    /// </summary>
    /// <remarks>
    /// Prints an error message for editors so that they can easily report errors to developers.
    /// </remarks>
    public class ErrorHandlingContentRenderer : IContentRenderer
    {
        private readonly MvcContentRenderer _mvcRenderer;
        private readonly bool _isDebugging;
        public ErrorHandlingContentRenderer(MvcContentRenderer mvcRenderer)
        {
            _mvcRenderer = mvcRenderer;
            _isDebugging = Debugger.IsAttached;
        }

        /// <summary>
        /// Renders the contentData using the wrapped renderer and catches common, non-critical exceptions.
        /// </summary>
        public async Task RenderAsync(IHtmlHelper helper, IContentData contentData, TemplateModel templateModel)
        {
            try
            {
                await _mvcRenderer.RenderAsync(helper, contentData, templateModel);
            }
            catch (NullReferenceException ex)
            {
                if (_isDebugging)
                {
                    //If debug="true" we assume a developer is making the request
                    throw;
                }
                HandlerError(helper, contentData, ex);
            }
            catch (ArgumentException ex)
            {
                if (_isDebugging)
                {
                    throw;
                }
                HandlerError(helper, contentData, ex);
            }
            catch (ApplicationException ex)
            {
                if (_isDebugging)
                {
                    throw;
                }
                HandlerError(helper, contentData, ex);
            }
            catch (InvalidOperationException ex)
            {
                if (_isDebugging)
                {
                    throw;
                }
                HandlerError(helper, contentData, ex);
            }
            catch (NotImplementedException ex)
            {
                if (_isDebugging)
                {
                    throw;
                }
                HandlerError(helper, contentData, ex);
            }
            catch (IOException ex)
            {
                if (_isDebugging)
                {
                    throw;
                }
                HandlerError(helper, contentData, ex);
            }
            catch (EPiServerException ex)
            {
                if (_isDebugging)
                {
                    throw;
                }
                HandlerError(helper, contentData, ex);
            }
        }

        private void HandlerError(IHtmlHelper helper, IContentData contentData, Exception renderingException)
        {
            if (helper.ViewContext?.HttpContext?.User?.HasEditAccess() ?? false)
            {
                var errorModel = new ContentRenderingErrorModel(contentData, renderingException);
                helper.RenderPartialAsync("TemplateError", errorModel).GetAwaiter().GetResult();
            }
        }
    }
}
