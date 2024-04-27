using Domain.Entities;
using Domain.ValueObjects;
using Infra.Context;
using Infra.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infra.Repositories;

public class DelivererRepository : Repository<Deliverer>, IDelivererRepository
{
    private readonly ILogger<DelivererRepository> _logger;
    public DelivererRepository(
        DmContext context,
        ILogger<DelivererRepository> logger) : base(context, logger)
    {
        _logger = logger;
    }

    public Deliverer? GetByPrimaryDocument(string document)
    {
        try
        {
            var cnpj = new CNPJ(document);
            
            var deliverer = _context.Deliverers.FirstOrDefault(x => !x.Excluded && x.PrimaryDocument == cnpj.Value);

            return deliverer;
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return null;
        }
    }

    public Deliverer? GetByCnh(string cnh)
    {
        try
        {
            var deliverer = _context.Deliverers.FirstOrDefault(x => !x.Excluded && x.Cnh == cnh);

            return deliverer;
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return null;
        }
    }
}