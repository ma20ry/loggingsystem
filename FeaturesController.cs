using AutoMapper;
using Commons.HealthExpenseManagement.Data.Entities.Permissions;
using Commons.HealthExpenseManagement.Helper.LoggingSystem;

using Commons.HealthExpenseManagment.Data.Entities.Users;
using identity.Dtos.Permissions;
using identity.Services.PermissionsServices.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.ComponentModel;
using static Community.CsharpSqlite.Sqlite3;
using static IronPython.Modules._ast;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace identity.Controllers.Permissions
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeaturesController : ControllerBase
    {
        private readonly IFeatureRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<FeaturesController> _Logger;
        private readonly ILoggingService<FeaturesController> _loggingService;
        private readonly IConfiguration _configuration;
       /* private readonly string _controllerName;*/


        public FeaturesController(IFeatureRepository featureRepository, IMapper mapper ,
            ILoggingService<FeaturesController> loggingService, ILogger<FeaturesController> logger, 
             IConfiguration configuration
    )
        {
            _repository = featureRepository;
            _mapper = mapper;
            _Logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _loggingService = new LoggingService<FeaturesController>(logger, _configuration);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFeatures(string? name, string? code, int pageNumber = 1, int pageSize = 10)
        {
          try {
                var (features, paginationMetadata) = await _repository.GetAllAsync(name, code, pageNumber, pageSize);
                if (features == null)
                {
                    string endpointName = HttpContext.Request.Path;
                     _loggingService.LogWarning<WarningException>(new WarningException($"No data found for"), endpointName);
                    return BadRequest("not found");
                }
            var featuresDto = new
            {
                data = _mapper.Map<IEnumerable<FeaturesDto>>(features),
                pagination = paginationMetadata
            };
                string successEndpointName = HttpContext.Request.Path;
                 _loggingService.LogGetSuccess<Feature>(successEndpointName);

                return Ok(featuresDto);
            }
            catch (DbUpdateException dbex)
            {
                var (features, paginationMetadata) = await _repository.GetAllAsync(name, code, pageNumber, pageSize);
                string endpointName = HttpContext.Request.Path;
                 _loggingService.LogGeneralError(dbex, features, endpointName);
                return BadRequest("An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while processing the request at {Endpoint}. Error Message: {Message}");
                return StatusCode(500, "An internal error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeatureById(Guid id)
        {
          try {
                var feature = await _repository.GetByIdAsync(id);
            if (id == null)
            {
                    string endpointName = HttpContext.Request.Path;
                    _loggingService.LogWarning<WarningException>(new WarningException($"No data found for ID: {id}"), endpointName);
                    return BadRequest("not found");
                }

            var featureDto = _mapper.Map<FeaturesDto>(feature);
                string successEndpointName = HttpContext.Request.Path;
                _loggingService.LogGetSuccess<Feature>(successEndpointName);
                return Ok(featureDto);
            }
            catch (DbUpdateException dbex)
            {
                string endpointName = HttpContext.Request.Path;
                _loggingService.LogGeneralError(dbex, id, endpointName);
                return BadRequest("An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while processing the request at {Endpoint}. Error Message: {Message}");
                return StatusCode(500, "An internal error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeature([FromBody] FeaturesForCreationDto featureForCreationDto)
        {
        try  { 
                if (featureForCreationDto == null)
            {
                string endpointName = HttpContext.Request.Path;
                _loggingService.LogWarning<WarningException>(new WarningException($"No data found "), endpointName);
                return BadRequest("not found");
            }

            var featureEntity = _mapper.Map<Feature>(featureForCreationDto);
            await _repository.AddAsync(featureEntity);

               /* if (await _repository.SaveChangesAsync())
                {
                    var featureToReturn = _mapper.Map<FeaturesDto>(featureEntity);

                }*/
                string successEndpointName = HttpContext.Request.Path;
                _loggingService.LogCreationSuccess(featureEntity, successEndpointName);
                return Ok(featureEntity);
            }
            catch (DbUpdateException dbex)
            { 
                string endpointName = HttpContext.Request.Path;
                _loggingService.LogGeneralError(dbex, featureForCreationDto, endpointName);
                return BadRequest("An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while processing the request at {Endpoint}. Error Message: {Message}");
                return StatusCode(500, "An internal error occurred while processing your request.");
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateFeature([FromBody] JsonPatchDocument<FeaturesForCreationDto> patchDoc, Guid id)
        {
           
                Feature contractGr = await _repository.GetByIdAsync(id);
                if (contractGr == null) {
                    string endpointName = HttpContext.Request.Path;
                    _loggingService.LogWarning<WarningException>(new WarningException($"No data found "), endpointName);
                    return NotFound("Feature Not Found or the ID you provided is invalid.");
                }

                var toPatch = _mapper.Map<FeaturesForCreationDto>(contractGr);
                patchDoc.ApplyTo(toPatch);
                Feature olddataclone = contractGr.Clone();
                _mapper.Map<FeaturesForCreationDto, Feature>(toPatch, contractGr);
            try
            {
               var  updateData = await _repository.GetByIdAsync(id);
                await _repository.SaveChangesAsync();
                string endpoint = HttpContext.Request.Path;
                // Log the successful update with old and new data
                _loggingService.LogUpdateSuccess(updateData, endpoint, olddataclone);
                return Ok("The Feature was successfully Updated .");
            }
            catch (DbUpdateException dbex)
            {
                string endpointName = HttpContext.Request.Path;
                _loggingService.LogGeneralError(dbex, id, endpointName);
                return BadRequest("An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while processing the request at {Endpoint}. Error Message: {Message}");
                return StatusCode(500, "An internal error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeature(Guid id)
        {
       try     {
                var feature = await _repository.GetByIdAsync(id);
            if (feature == null)
                {
                    string endpointName = HttpContext.Request.Path;
                    _loggingService.LogWarning<WarningException>(new WarningException($"No data found for ID: {id}"), endpointName);
                    return NotFound(); }

            _repository.DeleteAsync(feature);
            await _repository.SaveChangesAsync();
                string endpoint = HttpContext.Request.Path;
                _loggingService.LogDeleteSuccess(endpoint, feature);
                return NoContent();
            }
            catch (DbUpdateException dbex)
            {
                string endpointName = HttpContext.Request.Path;
                _loggingService.LogGeneralError(dbex, id, endpointName);
                return BadRequest("An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while processing the request at {Endpoint}. Error Message: {Message}");
                return StatusCode(500, "An internal error occurred while processing your request.");
            }
        }
          

    }
}
