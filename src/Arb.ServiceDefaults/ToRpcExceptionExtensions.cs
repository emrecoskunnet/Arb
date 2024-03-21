using Grpc.Core;
using TebaTech.SharedKernel;
using TebaTech.SharedKernel.Exceptions;
using TebaTech.SharedKernel.Interfaces;
using Status = Grpc.Core.Status;
using StatusCode = Grpc.Core.StatusCode;

namespace Paros.Infrastructure.Extensions;

public static class ToRpcExceptionExtensions
{
    public static RpcException ToRpcException(this DomainObjectAlreadyExistsException e, IAppLocalizer localizer,
        string keyObjectValue)
    {
        return new RpcException(
            new Status(StatusCode.AlreadyExists, localizer["'{0}' already exists", keyObjectValue]),
            MetadataBuild(e, keyObjectValue));
    }

    public static RpcException ToRpcException(this DomainObjectNotFoundException e, IAppLocalizer localizer,
        string keyObjectValue)
    {
        return new RpcException(
            new Status(StatusCode.NotFound, localizer["'{0}' not found", keyObjectValue]),
            MetadataBuild(e, keyObjectValue));
    }

    public static RpcException ToRpcException(this ArgumentException e, IAppLocalizer localizer)
    {
        return new RpcException(
            new Status(StatusCode.InvalidArgument, localizer["The provided request is invalid"]),
            new Metadata
            {
                { e.ParamName ?? "criteria", e.Message }
            });
    }

    public static RpcException ToRpcException(this Exception e, IAppLocalizer localizer)
    {
        return new RpcException(
            new Status(StatusCode.Unknown, localizer["An error occurred while processing the request"], e)
        );
    }

    private static Metadata MetadataBuild(DomainException e, string keyObjectValue)
    {
        Metadata metadata = new();
        foreach (var m in e.Metadata)
        {
            foreach (string v in m.values)
            {
                metadata.Add(m.propertyName, v ?? throw new InvalidOperationException());
            }
        }

        metadata.Add("criteria", keyObjectValue);
        return metadata;
    }
}
