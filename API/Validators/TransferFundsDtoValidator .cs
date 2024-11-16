using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using FluentValidation;

namespace API.Validators
{
    public class TransferFundsDtoValidator : AbstractValidator<TransferFundsDto>
    {
        public TransferFundsDtoValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Transfer amount must be greater than 0.");
        }
    }
}