using DemoCosmos.ConsoleApp.Models;
using Microsoft.Extensions.Logging;
using SDDev.Net.GenericRepository.Contracts.Repository;

namespace DemoCosmos.ConsoleApp.Services;

public class DemoDataSeeder
{
    private readonly IRepository<Account> _accountRepo;
    private readonly IRepository<Team> _teamRepo;
    private readonly IRepository<Player> _playerRepo;
    private readonly IRepository<Coach> _coachRepo;
    private readonly IRepository<Game> _gameRepo;
    private readonly ILogger<DemoDataSeeder> _logger;

    public DemoDataSeeder(
        IRepository<Account> accountRepo,
        IRepository<Team> teamRepo,
        IRepository<Player> playerRepo,
        IRepository<Coach> coachRepo,
        IRepository<Game> gameRepo,
        ILogger<DemoDataSeeder> logger)
    {
        _accountRepo = accountRepo;
        _teamRepo = teamRepo;
        _playerRepo = playerRepo;
        _coachRepo = coachRepo;
        _gameRepo = gameRepo;
        _logger = logger;
    }

    public async Task SeedDemoDataAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🌱 Starting demo data seeding...");

        // ========== TENANT HIERARCHY: Account → Teams → Players ↔ Coaches → Games ==========
        
        // 1. Create Account (Tenant Root)
        var account = await CreateAccountAsync("Distrito de Fútbol Metropolitano", cancellationToken);
        _logger.LogInformation("✅ Account created: {AccountId} - {Name}", account.Id, account.DisplayName);

        // 2. Create Teams within Account
        var team1 = await CreateTeamAsync(account.Id!.Value, "Equipo Rojo", cancellationToken);
        var team2 = await CreateTeamAsync(account.Id!.Value, "Equipo Azul", cancellationToken);
        _logger.LogInformation("✅ Teams created: {Team1} ({Team1Id}) | {Team2} ({Team2Id})", 
            team1.NameDisplay, team1.Id, team2.NameDisplay, team2.Id);

        // 3. Create Players within Account and assign to Teams
        var player1 = await CreatePlayerAsync(account.Id!.Value, "Juan Pérez", "juan@email.com", team1.Id!.Value, "Forward", cancellationToken);
        var player2 = await CreatePlayerAsync(account.Id!.Value, "Carlos López", "carlos@email.com", team1.Id!.Value, "Midfielder", cancellationToken);
        var player3 = await CreatePlayerAsync(account.Id!.Value, "Diego García", "diego@email.com", team2.Id!.Value, "Defender", cancellationToken);
        
        _logger.LogInformation("✅ Players created and assigned:");
        _logger.LogInformation("   - {Player1} → {Team1}", player1.FullName, team1.NameDisplay);
        _logger.LogInformation("   - {Player2} → {Team1}", player2.FullName, team1.NameDisplay);
        _logger.LogInformation("   - {Player3} → {Team2}", player3.FullName, team2.NameDisplay);

        // 4. Create Coaches within Account
        var coach1 = await CreateCoachAsync(account.Id!.Value, "Fernando Martínez", "fernando@email.com", "UEFA-2023", team1.Id!.Value, cancellationToken);
        var coach2 = await CreateCoachAsync(account.Id!.Value, "Roberto Hernández", "roberto@email.com", "UEFA-2022", team2.Id!.Value, cancellationToken);
        
        _logger.LogInformation("✅ Coaches created:");
        _logger.LogInformation("   - {Coach1} (License: {License1}) → {Team1}", coach1.FullName, coach1.LicenseNumber, team1.NameDisplay);
        _logger.LogInformation("   - {Coach2} (License: {License2}) → {Team2}", coach2.FullName, coach2.LicenseNumber, team2.NameDisplay);

        // 5. Create Games between Teams
        var game1 = await CreateGameAsync(account.Id!.Value, team1.Id!.Value, team2.Id!.Value, DateTime.UtcNow.AddDays(7), cancellationToken);
        var game2 = await CreateGameAsync(account.Id!.Value, team2.Id!.Value, team1.Id!.Value, DateTime.UtcNow.AddDays(14), cancellationToken);
        
        _logger.LogInformation("✅ Games scheduled:");
        _logger.LogInformation("   - {Team1} vs {Team2} on {Date1}", team1.NameDisplay, team2.NameDisplay, game1.ScheduledAt);
        _logger.LogInformation("   - {Team2} vs {Team1} on {Date2}", team2.NameDisplay, team1.NameDisplay, game2.ScheduledAt);

        _logger.LogInformation("✨ Demo data seeding completed successfully!");
    }

    // ===== Helper Methods (Insert + Read from DB) =====

    private async Task<Account> CreateAccountAsync(string displayName, CancellationToken cancellationToken)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            DisplayName = displayName,
            Name = displayName.ToLower().Replace(" ", "-"),
            IsActive = true,
            AuditMetadata = new AuditMetadata
            {
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                CreatedBy = "seeder",
                ModifiedBy = "seeder"
            }
        };

        // Insert into Cosmos
        await _accountRepo.Create(account);
        
        // ⚠️ BREAKPOINT: Account created in DB
        _logger.LogDebug("Account inserted with ID: {AccountId}, PartitionKey: {PartitionKey}", account.Id, account.PartitionKey);

        // Read back from DB to ensure it's persisted (for verification)
        var savedAccount = await _accountRepo.Get(account.Id!.Value, account.PartitionKey);
        
        // ⚠️ BREAKPOINT: Account retrieved from DB
        return savedAccount ?? account;
    }

    private async Task<Team> CreateTeamAsync(Guid accountId, string nameDisplay, CancellationToken cancellationToken)
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            NameDisplay = nameDisplay,
            IsActive = true,
            AuditMetadata = new AuditMetadata
            {
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                CreatedBy = "seeder",
                ModifiedBy = "seeder"
            }
        };

        // Insert into Cosmos
        await _teamRepo.Create(team);
        
        // ⚠️ BREAKPOINT: Team created in DB
        _logger.LogDebug("Team inserted with ID: {TeamId}, PartitionKey: {PartitionKey}, AccountId: {AccountId}", 
            team.Id, team.PartitionKey, team.AccountId);

        // Read back from DB
        var savedTeam = await _teamRepo.Get(team.Id!.Value, team.PartitionKey);
        
        // ⚠️ BREAKPOINT: Team retrieved from DB
        return savedTeam ?? team;
    }

    private async Task<Player> CreatePlayerAsync(
        Guid accountId, string fullName, string email, Guid teamId, string role, CancellationToken cancellationToken)
    {
        var player = new Player
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            FullName = fullName,
            Email = email,
            IsActive = true,
            BirthDate = DateTime.UtcNow.AddYears(-25),
            TeamAssignments = new List<TeamAssignment>
            {
                new TeamAssignment { TeamId = teamId, Role = role, JoinedAt = DateTime.UtcNow }
            },
            AuditMetadata = new AuditMetadata
            {
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                CreatedBy = "seeder",
                ModifiedBy = "seeder"
            }
        };

        // Insert into Cosmos
        await _playerRepo.Create(player);
        
        // ⚠️ BREAKPOINT: Player created in DB
        _logger.LogDebug("Player inserted with ID: {PlayerId}, PartitionKey: {PartitionKey}, AccountId: {AccountId}, TeamId: {TeamId}", 
            player.Id, player.PartitionKey, player.AccountId, teamId);

        // Read back from DB
        var savedPlayer = await _playerRepo.Get(player.Id!.Value, player.PartitionKey);
        
        // ⚠️ BREAKPOINT: Player retrieved from DB
        return savedPlayer ?? player;
    }

    private async Task<Coach> CreateCoachAsync(
        Guid accountId, string fullName, string email, string licenseNumber, Guid teamId, CancellationToken cancellationToken)
    {
        var coach = new Coach
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            FullName = fullName,
            Email = email,
            LicenseNumber = licenseNumber,
            IsActive = true,
            AuditMetadata = new AuditMetadata
            {
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                CreatedBy = "seeder",
                ModifiedBy = "seeder"
            }
        };

        // Insert into Cosmos
        await _coachRepo.Create(coach);
        
        // ⚠️ BREAKPOINT: Coach created in DB
        _logger.LogDebug("Coach inserted with ID: {CoachId}, PartitionKey: {PartitionKey}, AccountId: {AccountId}", 
            coach.Id, coach.PartitionKey, coach.AccountId);

        // Read back from DB
        var savedCoach = await _coachRepo.Get(coach.Id!.Value, coach.PartitionKey);
        
        // ⚠️ BREAKPOINT: Coach retrieved from DB
        return savedCoach ?? coach;
    }

    private async Task<Game> CreateGameAsync(
        Guid accountId, Guid homeTeamId, Guid awayTeamId, DateTime scheduledAt, CancellationToken cancellationToken)
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            HomeTeamId = homeTeamId,
            AwayTeamId = awayTeamId,
            ScheduledAt = scheduledAt,
            IsActive = true,
            AuditMetadata = new AuditMetadata
            {
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                CreatedBy = "seeder",
                ModifiedBy = "seeder"
            }
        };

        // Insert into Cosmos
        await _gameRepo.Create(game);
        
        // ⚠️ BREAKPOINT: Game created in DB
        _logger.LogDebug("Game inserted with ID: {GameId}, PartitionKey: {PartitionKey}, HomeTeam: {HomeTeamId}, AwayTeam: {AwayTeamId}", 
            game.Id, game.PartitionKey, game.HomeTeamId, game.AwayTeamId);

        // Read back from DB
        var savedGame = await _gameRepo.Get(game.Id!.Value, game.PartitionKey);
        
        // ⚠️ BREAKPOINT: Game retrieved from DB
        return savedGame ?? game;
    }
}