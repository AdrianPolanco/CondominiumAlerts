using CondominiumAlerts.CrossCutting.Behaviors;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Extensions;
using CondominiumAlerts.Features.Features.Condominiums.Join;
using CondominiumAlerts.Infrastructure.Persistence.Context;
using CondominiumAlerts.Infrastructure.Persistence.Repositories;
using FluentValidation;
using LightResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Polly;
using Serilog;
using System.Reflection;
using System.Reflection.Metadata;

namespace CondominiumAlerts.Tests.Unit.Features.Condominiums.Join
{

    public class JoinCondominiumTests
    {

        private readonly Mock<ISender> _mockSender;
        private readonly JoinCondominiumCommand _command;
        private ApplicationDbContext _context;
        private Repository<Condominium, Guid> condominiumRepo;
        private JoinCondominiumCommandHandler _handler;
        private Repository<CondominiumUser, Guid> condominiumUserRepo;

        public JoinCondominiumTests()
        {

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
         .UseNpgsql<ApplicationDbContext>("Host=ep-sweet-wave-a87bdkhv-pooler.eastus2.azure.neon.tech;Port=5432;Username=neondb_owner;Password=npg_cW8RGSzdx1vp;Database=neondb;SSL Mode=Require;Trust Server Certificate=true;\n")
            .Options;

            _context = new ApplicationDbContext(options);

             condominiumRepo = new Repository<Condominium, Guid>(_context);
            var userRepo = new Repository<User, string>(_context);
             condominiumUserRepo = new Repository<CondominiumUser, Guid>(_context);
            var validator = new JoinCondominiumValidator();
            var logger = NullLogger<JoinCondominiumCommand>.Instance;

            _handler = new JoinCondominiumCommandHandler(
                condominiumRepo,
                userRepo,
                condominiumUserRepo,
                validator,
                logger
            );

            _command = new();
        }

        [Fact]
        public async Task ShouldFail_When_CodeIsNull()
        {
            _command.CondominiumCode = null;
            _command.UserId = "OLrNj6uE0WWxeBMpHg9vf1ikirX2";


            Result<JoinCondominiumResponce> result = await _handler.Handle(_command,default);

            Assert.False(result.IsSuccess);
            Assert.True(!string.IsNullOrEmpty(result.Error.Message));
            Assert.True(result.IsFailed);
            Assert.Contains("The code was not passed", result.Error.Message);
        }

        [Fact]
        public async Task ShouldFail_When_CodeIsEmpty()
        {
            _command.CondominiumCode = "";
            _command.UserId = "OLrNj6uE0WWxeBMpHg9vf1ikirX2";


            Result<JoinCondominiumResponce> result = await _handler.Handle(_command, default);

            Assert.False(result.IsSuccess);
            Assert.True(!string.IsNullOrEmpty(result.Error.Message));
            Assert.True(result.IsFailed);
            Assert.Contains("The code was not passed", result.Error.Message);
        }

        [Fact]
        public async Task ShouldFail_When_CodeIsToShort()
        {
            _command.CondominiumCode = "qwerw";
            _command.UserId = "OLrNj6uE0WWxeBMpHg9vf1ikirX2";


            Result<JoinCondominiumResponce> result = await _handler.Handle(_command, default);

            Assert.False(result.IsSuccess);
            Assert.True(!string.IsNullOrEmpty(result.Error.Message));
            Assert.True(result.IsFailed);
            Assert.Contains("The code is to short, it must be 11 characters long", result.Error.Message);
        }


        [Fact]
        public async Task ShouldFail_When_CodeIsToLong()
        {
            _command.CondominiumCode = "qwerwetsrgerrhehstrhsdfb";
            _command.UserId = "OLrNj6uE0WWxeBMpHg9vf1ikirX2";


            Result<JoinCondominiumResponce> result = await _handler.Handle(_command, default);

            Assert.False(result.IsSuccess);
            Assert.True(!string.IsNullOrEmpty(result.Error.Message));
            Assert.True(result.IsFailed);
            Assert.Contains("The code is to long, it must be 11 characters long", result.Error.Message);
        }


        [Fact]
        public async Task ShouldFail_When_UserIdIsNull()
        {
            _command.CondominiumCode = "qwerwetsrgerrhehstrhsdfb";
            _command.UserId = null;


            Result<JoinCondominiumResponce> result = await _handler.Handle(_command, default);

            Assert.False(result.IsSuccess);
            Assert.True(!string.IsNullOrEmpty(result.Error.Message));
            Assert.True(result.IsFailed);
            Assert.Contains("The user was not especified", result.Error.Message);
        }

        [Fact]
        public async Task ShouldFail_When_UserIdIsEmpty()
        {
            _command.CondominiumCode = "ggggggggggg";
            _command.UserId = "";


            Result<JoinCondominiumResponce> result = await _handler.Handle(_command, default);

            Assert.False(result.IsSuccess);
            Assert.True(!string.IsNullOrEmpty(result.Error.Message));
            Assert.True(result.IsFailed);
            Assert.Contains("The user was not especified", result.Error.Message);
        }
        [Fact]
        public async Task ShouldFail_When_codeDosentBelongToAnyCondominium()
        {
            _command.CondominiumCode = "ggggggggggg";
            _command.UserId = "OLrNj6uE0WWxeBMpHg9vf1ikirX2";


            Result<JoinCondominiumResponce> result = await _handler.Handle(_command, default);

            Assert.False(result.IsSuccess);
            Assert.True(!string.IsNullOrEmpty(result.Error.Message));
            Assert.True(result.IsFailed);
            Assert.Equal("The Code ggggggggggg dosent belong to any condominium", result.Error.Message);
        }


        [Fact]
        public async Task ShouldFail_When_userDosentExits()
        {
            //Get data for making this test run
            _command.CondominiumCode = "tTd/uIvrthy";
            _command.UserId = "OLrNj6uE0WWxeBMpHg9vf1ikirX2";


            Result<JoinCondominiumResponce> result = await _handler.Handle(_command, default);

            Assert.False(result.IsSuccess);
            Assert.True(!string.IsNullOrEmpty(result.Error.Message));
            Assert.True(result.IsFailed);
            Assert.Equal("The user couldn't be founded", result.Error.Message);
        }
        [Fact]
        public async Task ShouldFail_When_userIsAlreadyPartOfThatCondominium()
        {
            //Get data for making this test run
            _command.CondominiumCode = "tTd/uIvrthy";
            _command.UserId = "OLrNj6uE0WWxeBMpHg9vf1ikirX2";

            // try to make a db entry so that this test can pass

    //      await  condominiumUserRepo.CreateAsync(new()
    //        {
    //            CondominiumId =
    //condominiumRepo.GetOneByFilterAsync(c => c.InviteCode == "tTd/uIvrthy").Result.Id,
    //            UserId = "OLrNj6uE0WWxeBMpHg9vf1ikirX2"
    //        });

            Result<JoinCondominiumResponce> result = await _handler.Handle(_command, default);

            Assert.False(result.IsSuccess);
            Assert.True(!string.IsNullOrEmpty(result.Error.Message));
            Assert.True(result.IsFailed);
            Assert.Contains("The user is already part of the condominium", result.Error.Message);
        }
    }
}
