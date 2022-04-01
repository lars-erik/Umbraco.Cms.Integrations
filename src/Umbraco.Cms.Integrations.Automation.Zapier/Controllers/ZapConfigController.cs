﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Controllers
{
    [PluginController("UmbracoCmsIntegrationsAutomationZapier")]
    public class ZapConfigController : UmbracoAuthorizedApiController
    {
        private readonly ZapConfigService _zapConfigService;

        private readonly ZapierService _zapierService;

        public ZapConfigController(ZapConfigService zapConfigService, ZapierService zapierService)
        {
            _zapConfigService = zapConfigService;

            _zapierService = zapierService;
        }

        [HttpGet]
        public IEnumerable<ContentTypeDto> GetContentTypes()
        {
            IContentTypeService contentTypeService = Services.ContentTypeService;

            var contentTypes = contentTypeService.GetAll();

            var configEntities = _zapConfigService.GetAll().Select(p => p.ContentTypeName);

            return contentTypes
                .Where(p => !configEntities.Contains(p.Name))
                .OrderBy(p => p.Name)
                .Select(p => new ContentTypeDto
                    {
                        Name = p.Name
                    });
        }

        [HttpPost]
        public string Add([FromBody] ContentConfigDto dto)
        {
            var getByNameResult = _zapConfigService.GetByName(dto.ContentTypeName);
            if (getByNameResult != null) return "A record for this content type already exists.";

            var result = _zapConfigService.Add(dto);
            
            return result;
        } 

        [HttpGet]
        public IEnumerable<ContentConfigDto> GetAll() => _zapConfigService.GetAll();

        [HttpDelete]
        public string Delete(int id) => _zapConfigService.Delete(id);

        [HttpPost]
        public async Task<string> TriggerAsync([FromBody] ContentConfigDto dto)
        {
            return await _zapierService.TriggerAsync(dto.WebHookUrl,
                new Dictionary<string, string> { { Constants.Content.Name, dto.ContentTypeName } });
        }
    }
}
