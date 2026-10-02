using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniAccounting.Domain.Interfaces;
using MiniAccounting.Infrastructure.Persistence.Context;
using MiniAccounting.Infrastructure.Repositories;
using MiniAccounting.Infrastructure.Persistence;

namespace MiniAccounting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<MiniAccountingDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAccountRepository, AccountRepository>();

        services.AddScoped<
            IJournalEntryRepository,
            JournalEntryRepository>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}   