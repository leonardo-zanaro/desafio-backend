using Domain.Entities;
using Domain.ValueObjects;
using Infra.Context;
using Infra.Repositories.Interfaces;

namespace Infra.Repositories;

public class DelivererRepository : Repository<Deliverer>, IDelivererRepository
{
    public DelivererRepository(DmContext context) : base(context)
    {
    }

    public Deliverer? GetByPrimaryDocument(string document)
    {
        var cnpj = new CNPJ(document);
        
        var deliverer = _context.Deliverers.FirstOrDefault(x => !x.Excluded && x.PrimaryDocument == cnpj.Value);

        return deliverer;
    }

    public Deliverer? GetByCnh(string cnh)
    {
        var deliverer = _context.Deliverers.FirstOrDefault(x => !x.Excluded && x.Cnh == cnh);

        return deliverer;
    }
}