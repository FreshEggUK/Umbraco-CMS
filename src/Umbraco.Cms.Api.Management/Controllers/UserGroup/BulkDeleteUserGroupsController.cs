﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Management.ViewModels.UserGroup;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.OperationStatus;
using Umbraco.Cms.Web.Common.Authorization;

namespace Umbraco.Cms.Api.Management.Controllers.UserGroup;

[ApiVersion("1.0")]
public class BulkDeleteUserGroupsController : UserGroupControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserGroupService _userGroupService;

    public BulkDeleteUserGroupsController(IAuthorizationService authorizationService, IUserGroupService userGroupService)
    {
        _authorizationService = authorizationService;
        _userGroupService = userGroupService;
    }

    [HttpDelete]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BulkDelete(DeleteUserGroupsRequestModel model)
    {
        AuthorizationResult authorizationResult = await _authorizationService.AuthorizeAsync(User, model.UserGroupIds,
            $"New{AuthorizationPolicies.UserBelongsToUserGroupInRequest}");

        if (!authorizationResult.Succeeded)
        {
            return Forbidden();
        }

        Attempt<UserGroupOperationStatus> result = await _userGroupService.DeleteAsync(model.UserGroupIds);

        return result.Success
            ? Ok()
            : UserGroupOperationStatusResult(result.Result);
    }
}