using SolicitorFinder.Application.Report.Dtos;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Report;

public sealed record class GetReportsQuery(int? TopCount) : IRequest<ReportsDto>;