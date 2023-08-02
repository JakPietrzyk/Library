using System.Net;
using AutoFixture;
using AutoMapper;
using HistoryRental.Controllers;
using HistoryRental.Dtos;
using HistoryRental.Mappers;
using HistoryRental.Model;
using HistoryRental.Services;
using HistoryRental.Clients;
using HistoryRental.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using Xunit;


namespace Rental{
    public class Testclass
    {
        private readonly Fixture _fixture;
        private RentalController? _controller; 
        private Mock<IHistoryRentalService> _libraryService;
        private Mock<IBooksClient> _booksClientMocked;
        private Mock<IRentalClient> _rentalClientMocked;
        public Testclass()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _libraryService = new Mock<IHistoryRentalService>();
            _booksClientMocked = new Mock<IBooksClient>();
            _rentalClientMocked = new Mock<IRentalClient>();
        }
        
    }
}



