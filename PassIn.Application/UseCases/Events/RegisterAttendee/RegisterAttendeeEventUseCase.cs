using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using System.Net.Mail;

namespace PassIn.Application.UseCases.Events.RegisterAttendee
{
    public class RegisterAttendeeEventUseCase
    {
        private readonly PassInDbContext _dbContext;
        public RegisterAttendeeEventUseCase()
        {

            _dbContext = new PassInDbContext();

        }

        public ResponseRegisterJson Execute(Guid eventId, RequestRegisterEventJson request)
        {
            

            var dbContext = new PassInDbContext();

            Validate(eventId, request);

            var entity = new Infrastructure.Entities.Attendee
            {
                Email = request.Email,
                Name = request.Name,
                Event_Id = eventId,
                Created_At = DateTime.UtcNow

            };

            _dbContext.Attendees.Add(entity);
            _dbContext.SaveChanges();

            return new ResponseRegisterJson
            {
                Id = entity.Id
            };
        }

        private void Validate(Guid eventId, RequestRegisterEventJson request)
        {
            var existEvent = _dbContext.Events.Find(eventId);

            if (existEvent is null)
                throw new NotFoundException("An event with this id does not exist.");

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ErrorOnValidationException("The name is invalid.");
            }
            
            var emailIsValid = EmailIsValid(request.Email); ;
            if(emailIsValid == false)
            {
                throw new ErrorOnValidationException("The e-mail is invalid.");
            }

            var attendeeAlreadyRegistered = _dbContext
                .Attendees
                .Any(attendee => attendee.Email.Equals(request.Email) && attendee.Event_Id == eventId);

            if (attendeeAlreadyRegistered)
            {
                throw new ConflictException("You can not register twice on the same event.");
            }

            var attendeesForEvent = _dbContext.Attendees.Count(attendee => attendee.Event_Id == eventId);
        
            if(attendeesForEvent == existEvent.Maximum_Attendees)
            {
                throw new ErrorOnValidationException("There is no room for this event.");
            }
        }

        private bool EmailIsValid(string mail)
        {
            try
            { 
                new MailAddress(mail);
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
