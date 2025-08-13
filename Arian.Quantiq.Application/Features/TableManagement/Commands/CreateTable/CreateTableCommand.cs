using Arian.Quantiq.Application.DTOs.TableManagement;
using Arian.Quantiq.Domain.Common.Results;
using MediatR;

namespace Arian.Quantiq.Application.Features.TableManagement.Commands.CreateTable;

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