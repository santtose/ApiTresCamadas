using FluentValidation.Results;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevIO.Business.Models
{
    public abstract class Entity
    {
        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        [NotMapped]
        public ValidationResult ValidationResult { get; protected set; }

        public virtual bool EhValido()
        {
            throw new NotImplementedException();
        }
    }
}
