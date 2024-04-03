using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;
using System.Data.SqlTypes;

namespace PassIn.Application.UseCases.Chekins.DoCheckin
{
    public class DoAttendeeCheckinUseCase
    {
        private readonly PassInDbContext _dbContext;

        public DoAttendeeCheckinUseCase()
        {
            _dbContext = new PassInDbContext();
        }

        public ResponseRegisterJson Execute(Guid attendeeId)
        {
            Validate(attendeeId);

            var entity = new CheckIn
            {
                Attendee_Id = attendeeId,
                Created_at = DateTime.UtcNow,
            };

            _dbContext.CheckIns.Add(entity);
            _dbContext.SaveChanges();

            return new ResponseRegisterJson
            {
                Id = entity.Id,
            };
        }

        private void Validate(Guid attendeeId)
        {
            var existAttendee = _dbContext.Attendees.Any(attendee => attendee.Id == attendeeId);
            if(existAttendee == false)
            {
                throw new NotFoundException("The attendee with this Id was not found.");
            }

            var existCheckin = _dbContext.CheckIns.Any(ch => ch.Attendee_Id == attendeeId);
            if(existCheckin)
            {
                throw new ConflictException("Attendee can't do check-in twice in the same event.");
            }
        }
    }
}
