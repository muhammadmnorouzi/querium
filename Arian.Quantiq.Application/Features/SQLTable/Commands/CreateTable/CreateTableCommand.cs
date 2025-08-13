using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Domain.Common.Results;
using MediatR;

namespace Arian.Quantiq.Application.Features.SQLTable.Commands.CreateTable;

/// <summary>
/// Represents a command to create a new table in the database.
/// </summary>
public class CreateTableCommand : IRequest<ApplicationResult<AppVoid>>
{
    /// <summary>
    /// Gets or sets the table creation DTO
    /// </summary>
    public CreateTableDTO CreateTableDTO { get; set; } = null!;
}