using BankRUs.Application.GuardClause;
using BankRUs.Application.Services.AuditLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.MakeDepositToBankAccount.Guards
{
    public static class TransactionGuardExtension
    {
        public static decimal NegativeAmount(this IGuardClause guardClause, decimal input)
        {
            return Negative(guardClause, input, 0);
        }

        public static decimal ZeroAmount(this IGuardClause guardClause, decimal input)
        {
            return Zero(guardClause, input, 0);
        }

        public static decimal RoundToNearestHundredth(
            this IGuardClause guardClause, 
            decimal input, 
            IAuditLogger auditLogger)
        {
            decimal roundedInput = decimal.Round(input, 2, MidpointRounding.AwayFromZero);
            if (input != roundedInput)
            {
                auditLogger.Log(string.Format("Rounded {0} to {1}", input, roundedInput));
                return roundedInput;
            }

            return input;
        }

        private static T Negative<T>(this IGuardClause _, T input, T compareTo)
        {
            Comparer<T> comparer = Comparer<T>.Default;

            if (comparer.Compare(input, compareTo) < 0)
            {
                throw new ArgumentException("Transaction amount cannot be negative");
            }

            return input;
        }

        private static T Zero<T>(this IGuardClause _, T input, T compareTo)
        {
            Comparer<T> comparer = Comparer<T>.Default;

            if (comparer.Compare(input, compareTo) == 0)
            {
                throw new ArgumentException("Transaction amount must be greater than zero");
            }

            return input;
        }
    }
}
