using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;

namespace TaxiCallCenter.MVP.WpfApp.Extensions
{
    public class StateMachineConditionsBuilder<TState, TTrigger, TArg0>
    {
        private readonly List<Tuple<Func<TArg0, Boolean>, TState>> conditions = new List<Tuple<Func<TArg0, Boolean>, TState>>();
        private Boolean defaultStateSet = false;
        private TState defaultState;

        public StateMachineConditionsBuilder<TState, TTrigger, TArg0> Case(Func<TArg0, Boolean> condition, TState state)
        {
            this.conditions.Add(Tuple.Create(condition, state));
            return this;
        }

        public StateMachineConditionsBuilder<TState, TTrigger, TArg0> Default(TState state)
        {
            this.defaultStateSet = true;
            this.defaultState = state;
            return this;
        }

        public Func<TArg0, TState> CompileSelector()
        {
            if (!this.defaultStateSet)
            {
                throw new InvalidOperationException();
            }

            return x =>
            {
                foreach (var condition in this.conditions)
                {
                    if (condition.Item1(x))
                    {
                        return condition.Item2;
                    }
                }

                return this.defaultState;
            };
        }
    }

    public static class StateMachineExtensions
    {
        public static StateMachine<TState, TTrigger>.StateConfiguration PermitWhen<TState, TTrigger, TArg0>(
            this StateMachine<TState, TTrigger>.StateConfiguration configuration,
            StateMachine<TState, TTrigger>.TriggerWithParameters<TArg0> trigger,
            Action<StateMachineConditionsBuilder<TState, TTrigger, TArg0>> conditionsBuilder)
        {
            var builder = new StateMachineConditionsBuilder<TState, TTrigger, TArg0>();
            conditionsBuilder(builder);

            return configuration.PermitDynamic(trigger, builder.CompileSelector());
        }
    }
}
