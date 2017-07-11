using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using TaxiCallCenter.MVP.WpfApp.Extensions;
using TaxiCallCenter.MVP.WpfApp.Resources;

namespace TaxiCallCenter.MVP.WpfApp.Models
{
    public interface ISpeechSubsystem
    {
        Task SpeakAsync(String text);

        void SetRecognitionMode(String mode);

        void StopCommunication();
    }

    public interface ILogger
    {
        void LogEvent(String eventText);
    }

    public interface IOrdersService
    {
        Task CreateOrderAsync(OrderInfo order);
    }

    public class OrderStateMachine
    {
        private readonly ISpeechSubsystem speechSubsystem;
        private readonly ILogger logger;
        private readonly IOrdersService ordersService;

        private readonly StateMachine<State, Trigger> stateMachine;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<String> processResponseTrigger;

        private Address addressFrom;
        private Address addressTo;
        private String date;
        private String time;
        private String phone;
        private String additionalInfo;

        public OrderStateMachine(ISpeechSubsystem speechSubsystem, ILogger logger, IOrdersService ordersService)
        {
            this.speechSubsystem = speechSubsystem;
            this.logger = logger;
            this.ordersService = ordersService;

            this.stateMachine = new StateMachine<State, Trigger>(State.Initial);
            this.processResponseTrigger = this.stateMachine.SetTriggerParameters<String>(Trigger.ProcessResponse);

            this.stateMachine.Configure(State.Initial)
                .Permit(Trigger.Proceed, State.Greeting);


            #region Greeting

            this.stateMachine.Configure(State.Greeting)
                .OnEntryAsync(this.Greeting_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidYes, State.OrderFrom)
                    .Case(this.IsValidNo, State.NotOrder)
                    .Default(State.GreetingFailed01))
                .Permit(Trigger.RecognitionError, State.GreetingFailed01);

            this.stateMachine.Configure(State.GreetingFailed01)
                .OnEntryAsync(this.GreetingFailed01_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidYes, State.OrderFrom)
                    .Case(this.IsValidNo, State.NotOrder)
                    .Default(State.GreetingFailed02))
                .Permit(Trigger.RecognitionError, State.GreetingFailed02);

            this.stateMachine.Configure(State.GreetingFailed02)
                .OnEntryAsync(this.GreetingFailed02_Entry)
                .Permit(Trigger.Proceed, State.Aborted);

            #endregion

            #region NotOrder

            this.stateMachine.Configure(State.NotOrder)
                .OnEntryAsync(this.NotOrder_Entry)
                .Permit(Trigger.Proceed, State.Aborted);

            #endregion

            #region OrderFrom

            this.stateMachine.Configure(State.OrderFrom)
                .OnEntryAsync(this.OrderFrom_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidAddress, State.OrderFromConfirm)
                    .Default(State.OrderFromFailed01))
                .Permit(Trigger.RecognitionError, State.OrderFromFailed01);

            this.stateMachine.Configure(State.OrderFromFailed01)
                .OnEntryAsync(this.OrderFromFailed01_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidAddress, State.OrderFromConfirm)
                    .Default(State.OrderFromFailed02))
                .Permit(Trigger.RecognitionError, State.OrderFromFailed02);

            this.stateMachine.Configure(State.OrderFromFailed02)
                .OnEntryAsync(this.OrderFromFailed02_Entry)
                .Permit(Trigger.Proceed, State.Aborted);

            #endregion

            #region OrderFromConfirm

            this.stateMachine.Configure(State.OrderFromConfirm)
                .OnEntryFromAsync(this.processResponseTrigger, this.OrderFromConfirm_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidYes, State.OrderTo)
                    .Case(this.IsValidNo, State.OrderFromNotConfirmed)
                    .Default(State.OrderFromConfirmFailed01))
                .Permit(Trigger.RecognitionError, State.OrderFromConfirmFailed01);

            this.stateMachine.Configure(State.OrderFromConfirmFailed01)
                .OnEntryAsync(this.OrderFromConfirmFailed01_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidYes, State.OrderTo)
                    .Case(this.IsValidNo, State.OrderFromNotConfirmed)
                    .Default(State.OrderFromConfirmFailed02))
                .Permit(Trigger.RecognitionError, State.OrderFromConfirmFailed02);

            this.stateMachine.Configure(State.OrderFromConfirmFailed02)
                .OnEntryAsync(this.OrderFromConfirmFailed02_Entry)
                .Permit(Trigger.Proceed, State.Aborted);

            this.stateMachine.Configure(State.OrderFromNotConfirmed)
                .OnEntryAsync(this.OrderFromNotConfirmed_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidAddress, State.OrderFromConfirm)
                    .Default(State.OrderFromFailed01))
                .Permit(Trigger.RecognitionError, State.OrderFromFailed01);

            #endregion

            #region OrderTo

            this.stateMachine.Configure(State.OrderTo)
                .OnEntryAsync(this.OrderTo_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidAddress, State.OrderToConfirm)
                    .Default(State.OrderToFailed01))
                .Permit(Trigger.RecognitionError, State.OrderToFailed01);

            this.stateMachine.Configure(State.OrderToFailed01)
                .OnEntryAsync(this.OrderToFailed01_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidAddress, State.OrderToConfirm)
                    .Default(State.OrderToFailed02))
                .Permit(Trigger.RecognitionError, State.OrderToFailed02);

            this.stateMachine.Configure(State.OrderToFailed02)
                .OnEntryAsync(this.OrderToFailed02_Entry)
                .Permit(Trigger.Proceed, State.Aborted);

            #endregion

            #region OrderToConfirm

            this.stateMachine.Configure(State.OrderToConfirm)
                .OnEntryFromAsync(this.processResponseTrigger, this.OrderToConfirm_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidYes, State.OrderNowConfirm)
                    .Case(this.IsValidNo, State.OrderToNotConfirmed)
                    .Default(State.OrderToConfirmFailed01))
                .Permit(Trigger.RecognitionError, State.OrderToConfirmFailed01);

            this.stateMachine.Configure(State.OrderToConfirmFailed01)
                .OnEntryAsync(this.OrderToConfirmFailed01_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidYes, State.OrderNowConfirm)
                    .Case(this.IsValidNo, State.OrderToNotConfirmed)
                    .Default(State.OrderToConfirmFailed02))
                .Permit(Trigger.RecognitionError, State.OrderToConfirmFailed02);

            this.stateMachine.Configure(State.OrderToConfirmFailed02)
                .OnEntryAsync(this.OrderToConfirmFailed02_Entry)
                .Permit(Trigger.Proceed, State.Aborted);

            this.stateMachine.Configure(State.OrderToNotConfirmed)
                .OnEntryAsync(this.OrderToNotConfirmed_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidAddress, State.OrderToConfirm)
                    .Default(State.OrderToFailed01))
                .Permit(Trigger.RecognitionError, State.OrderToFailed01);

            #endregion

            #region OrderNowConfirm

            this.stateMachine.Configure(State.OrderNowConfirm)
                .OnEntryAsync(this.OrderNowConfirm_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidYes, State.OrderNowConfirmApply)
                    .Case(this.IsValidNo, State.OrderTodayConfirm)
                    .Default(State.OrderNowConfirmFailed01))
                .Permit(Trigger.RecognitionError, State.OrderNowConfirmFailed01);

            this.stateMachine.Configure(State.OrderNowConfirmFailed01)
                .OnEntryAsync(this.OrderNowConfirmFailed01_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidYes, State.OrderNowConfirmApply)
                    .Case(this.IsValidNo, State.OrderTodayConfirm)
                    .Default(State.OrderNowConfirmFailed02))
                .Permit(Trigger.RecognitionError, State.OrderNowConfirmFailed02);

            this.stateMachine.Configure(State.OrderNowConfirmFailed02)
                .OnEntryAsync(this.OrderNowConfirmFailed02_Entry)
                .Permit(Trigger.Proceed, State.Aborted);

            this.stateMachine.Configure(State.OrderNowConfirmApply)
                .OnEntryAsync(this.OrderNowConfirmApply_Entry)
                .Permit(Trigger.Proceed, State.OrderCallerPhoneConfirm);

            #endregion

            #region OrderTodayConfirm

            this.stateMachine.Configure(State.OrderTodayConfirm)
                .OnEntryAsync(this.OrderTodayConfirm_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidYes, State.OrderTodayTime)
                    .Case(this.IsValidNo, State.OrderCustomDateTime)
                    .Default(State.OrderTodayConfirmFailed01))
                .Permit(Trigger.RecognitionError, State.OrderTodayConfirmFailed01);

            this.stateMachine.Configure(State.OrderTodayConfirmFailed01)
                .OnEntryAsync(this.OrderTodayConfirmFailed01_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidYes, State.OrderTodayTime)
                    .Case(this.IsValidNo, State.OrderCustomDateTime)
                    .Default(State.OrderTodayConfirmFailed02))
                .Permit(Trigger.RecognitionError, State.OrderTodayConfirmFailed02);

            this.stateMachine.Configure(State.OrderTodayConfirmFailed02)
                .OnEntryAsync(this.OrderTodayConfirmFailed02_Entry)
                .Permit(Trigger.Proceed, State.Aborted);

            #endregion

            #region OrderTodayTime

            this.stateMachine.Configure(State.OrderTodayTime)
                .OnEntryAsync(this.OrderTodayTime_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidTime, State.OrderTodayTimeApply)
                    .Default(State.OrderTodayTimeFailed01))
                .Permit(Trigger.RecognitionError, State.OrderTodayTimeFailed01);

            this.stateMachine.Configure(State.OrderTodayTimeFailed01)
                .OnEntryAsync(this.OrderTodayTimeFailed01_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidTime, State.OrderTodayTimeApply)
                    .Default(State.OrderTodayTimeFailed02))
                .Permit(Trigger.RecognitionError, State.OrderTodayTimeFailed02);

            this.stateMachine.Configure(State.OrderTodayTimeFailed02)
                .OnEntryAsync(this.OrderTodayTimeFailed02_Entry)
                .Permit(Trigger.Proceed, State.Aborted);

            this.stateMachine.Configure(State.OrderTodayTimeApply)
                .OnEntryFromAsync(this.processResponseTrigger, this.OrderTodayTimeApply_Entry)
                .Permit(Trigger.Proceed, State.OrderCallerPhoneConfirm);

            #endregion

            #region OrderCustomDateTime

            this.stateMachine.Configure(State.OrderCustomDateTime)
                .OnEntryAsync(this.OrderCustomDateTime_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidDateTime, State.OrderCustomDateTimeApply)
                    .Default(State.OrderCustomDateTimeFailed01))
                .Permit(Trigger.RecognitionError, State.OrderCustomDateTimeFailed01);

            this.stateMachine.Configure(State.OrderCustomDateTimeFailed01)
                .OnEntryAsync(this.OrderCustomDateTimeFailed01_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidDateTime, State.OrderCustomDateTimeApply)
                    .Default(State.OrderCustomDateTimeFailed02))
                .Permit(Trigger.RecognitionError, State.OrderCustomDateTimeFailed02);

            this.stateMachine.Configure(State.OrderCustomDateTimeFailed02)
                .OnEntryAsync(this.OrderCustomDateTimeFailed02_Entry)
                .Permit(Trigger.Proceed, State.Aborted);

            this.stateMachine.Configure(State.OrderCustomDateTimeApply)
                .OnEntryFromAsync(this.processResponseTrigger, this.OrderCustomDateTimeApply_Entry)
                .Permit(Trigger.Proceed, State.OrderCallerPhoneConfirm);

            #endregion

            #region CallerPhoneConfirm

            this.stateMachine.Configure(State.OrderCallerPhoneConfirm)
                .OnEntryAsync(this.OrderCallerPhoneConfirm_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidYes, State.OrderCallerPhoneConfirmApply)
                    .Case(this.IsValidNo, State.OrderPhoneRequest)
                    .Default(State.OrderCallerPhoneConfirmFailed01))
                .Permit(Trigger.RecognitionError, State.OrderCallerPhoneConfirmFailed01);

            this.stateMachine.Configure(State.OrderCallerPhoneConfirmFailed01)
                .OnEntryAsync(this.OrderCallerPhoneConfirmFailed01_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidYes, State.OrderCallerPhoneConfirmApply)
                    .Case(this.IsValidNo, State.OrderPhoneRequest)
                    .Default(State.OrderCallerPhoneConfirmFailed02))
                .Permit(Trigger.RecognitionError, State.OrderCallerPhoneConfirmFailed02);

            this.stateMachine.Configure(State.OrderCallerPhoneConfirmFailed02)
                .OnEntryAsync(this.OrderCallerPhoneConfirmFailed02_Entry)
                .Permit(Trigger.Proceed, State.Aborted);

            this.stateMachine.Configure(State.OrderCallerPhoneConfirmApply)
                .OnEntryAsync(this.OrderCallerPhoneConfirmApply_Entry)
                .Permit(Trigger.Proceed, State.OrderAdditionalInfoRequest);

            #endregion

            #region PhoneRequest

            this.stateMachine.Configure(State.OrderPhoneRequest)
                .OnEntryAsync(this.OrderPhoneRequest_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidPhone, State.OrderPhoneRequestApply)
                    .Default(State.OrderPhoneRequestFailed01))
                .Permit(Trigger.RecognitionError, State.OrderPhoneRequestFailed01);

            this.stateMachine.Configure(State.OrderPhoneRequestFailed01)
                .OnEntryAsync(this.OrderPhoneRequestFailed01_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidPhone, State.OrderPhoneRequestApply)
                    .Default(State.OrderPhoneRequestFailed02))
                .Permit(Trigger.RecognitionError, State.OrderPhoneRequestFailed02);

            this.stateMachine.Configure(State.OrderPhoneRequestFailed02)
                .OnEntryAsync(this.OrderPhoneRequestFailed02_Entry)
                .Permit(Trigger.Proceed, State.Aborted);

            this.stateMachine.Configure(State.OrderPhoneRequestApply)
                .OnEntryFromAsync(this.processResponseTrigger, this.OrderPhoneRequestApply_Entry)
                .Permit(Trigger.Proceed, State.OrderAdditionalInfoRequest);

            #endregion

            #region AdditionalInfoRequest

            this.stateMachine.Configure(State.OrderAdditionalInfoRequest)
                .OnEntryAsync(this.OrderAdditionalInfoRequest_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidYes, State.OrderAdditionalInfo)
                    .Case(this.IsValidNo, State.OrderAdditionalInfoNoneApply)
                    .Default(State.OrderAdditionalInfoFailed01))
                .Permit(Trigger.RecognitionError, State.OrderAdditionalInfoRequestFailed01);

            this.stateMachine.Configure(State.OrderAdditionalInfoRequestFailed01)
                .OnEntryAsync(this.OrderAdditionalInfoRequestFailed01_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidYes, State.OrderAdditionalInfo)
                    .Case(this.IsValidNo, State.OrderAdditionalInfoNoneApply)
                    .Default(State.OrderAdditionalInfoFailed02))
                .Permit(Trigger.RecognitionError, State.OrderAdditionalInfoRequestFailed02);

            this.stateMachine.Configure(State.OrderAdditionalInfoRequestFailed02)
                .OnEntryAsync(this.OrderAdditionalInfoRequestFailed02_Entry)
                .Permit(Trigger.Proceed, State.Aborted);

            this.stateMachine.Configure(State.OrderAdditionalInfoNoneApply)
                .OnEntryAsync(this.OrderAdditionalInfoNoneApply_Entry)
                .Permit(Trigger.Proceed, State.OrderCreated);

            #endregion

            #region AdditionalInfo

            this.stateMachine.Configure(State.OrderAdditionalInfo)
                .OnEntryAsync(this.OrderAdditionalInfo_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidAdditionalInfo, State.OrderAdditionalInfoApply)
                    .Default(State.OrderAdditionalInfoFailed01))
                .Permit(Trigger.RecognitionError, State.OrderAdditionalInfoFailed01);

            this.stateMachine.Configure(State.OrderAdditionalInfoFailed01)
                .OnEntryAsync(this.OrderAdditionalInfoFailed01_Entry)
                .PermitWhen(this.processResponseTrigger, builder => builder
                    .Case(this.IsValidAdditionalInfo, State.OrderAdditionalInfoApply)
                    .Default(State.OrderAdditionalInfoFailed02))
                .Permit(Trigger.RecognitionError, State.OrderAdditionalInfoFailed02);

            this.stateMachine.Configure(State.OrderAdditionalInfoFailed02)
                .OnEntryAsync(this.OrderAdditionalInfoFailed02_Entry)
                .Permit(Trigger.Proceed, State.Aborted);

            this.stateMachine.Configure(State.OrderAdditionalInfoApply)
                .OnEntryFromAsync(this.processResponseTrigger, this.OrderAdditionalInfoApply_Entry)
                .Permit(Trigger.Proceed, State.OrderCreated);

            #endregion

            #region OrderCreated

            this.stateMachine.Configure(State.OrderCreated)
                .OnEntryAsync(this.OrderCreated_Entry)
                .Permit(Trigger.Proceed, State.Complete);

            #endregion

            this.stateMachine.Configure(State.Aborted)
                .OnEntryAsync(this.Aborted_Entry);

            this.stateMachine.Configure(State.Complete)
                .OnEntryAsync(this.Complete_Entry);
        }
        public async Task Initialize()
        {
            if (this.stateMachine.IsInState(State.Initial))
            {
                await this.stateMachine.FireAsync(Trigger.Proceed);
                this.logger.LogEvent($"Switching to state {this.stateMachine.State}");
            }
        }

        public async Task ProcessResponseAsync(String text)
        {
            await this.stateMachine.FireAsync(this.processResponseTrigger, text);
            this.logger.LogEvent($"Switching to state {this.stateMachine.State}");
            while (this.stateMachine.PermittedTriggers.Contains(Trigger.Proceed))
            {
                await this.stateMachine.FireAsync(Trigger.Proceed);
                this.logger.LogEvent($"Switching to state {this.stateMachine.State}");
            }
        }

        public async Task ProcessRecognitionFailure()
        {
            await this.stateMachine.FireAsync(Trigger.RecognitionError);
            this.logger.LogEvent($"Switching to state {this.stateMachine.State}");
            while (this.stateMachine.PermittedTriggers.Contains(Trigger.Proceed))
            {
                await this.stateMachine.FireAsync(Trigger.Proceed);
                this.logger.LogEvent($"Switching to state {this.stateMachine.State}");
            }
        }

        private async Task Greeting_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.Greeting01);
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task GreetingFailed01_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.GreetingFailed01);
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task GreetingFailed02_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.GreetingFailed02);
        }

        private async Task NotOrder_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.NotOrder01);
        }

        private async Task OrderFrom_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderFrom01);
            this.speechSubsystem.SetRecognitionMode("maps");
        }

        private async Task OrderFromFailed01_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderFromFailed01);
            this.speechSubsystem.SetRecognitionMode("maps");
        }

        private async Task OrderFromFailed02_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderFromFailed02);
        }

        private async Task OrderFromConfirm_Entry(String address)
        {
            this.addressFrom = AddressParser.ParseAddress(address);
            await this.speechSubsystem.SpeakAsync(String.Format(OrderDialogResources.OrderFromConfirm01, this.addressFrom));
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task OrderFromConfirmFailed01_Entry()
        {
            await this.speechSubsystem.SpeakAsync(String.Format(OrderDialogResources.OrderFromConfirmFailed01, this.addressFrom));
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task OrderFromConfirmFailed02_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderFromConfirmFailed02);
        }

        private async Task OrderFromNotConfirmed_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderFromNotConfirmed01);
            this.speechSubsystem.SetRecognitionMode("maps");
        }

        private async Task OrderTo_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderTo01);
            this.speechSubsystem.SetRecognitionMode("maps");
        }

        private async Task OrderToFailed01_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderToFailed01);
            this.speechSubsystem.SetRecognitionMode("maps");
        }

        private async Task OrderToFailed02_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderToFailed02);
        }

        private async Task OrderToConfirm_Entry(String address)
        {
            this.addressTo = AddressParser.ParseAddress(address);
            await this.speechSubsystem.SpeakAsync(String.Format(OrderDialogResources.OrderToConfirm01, this.addressTo));
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task OrderToConfirmFailed01_Entry()
        {
            await this.speechSubsystem.SpeakAsync(String.Format(OrderDialogResources.OrderToConfirmFailed01, this.addressTo));
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task OrderToConfirmFailed02_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderToConfirmFailed02);
        }

        private async Task OrderToNotConfirmed_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderToNotConfirmed01);
            this.speechSubsystem.SetRecognitionMode("maps");
        }

        private async Task OrderNowConfirm_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderNowConfirm01);
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task OrderNowConfirmFailed01_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderNowConfirmFailed01);
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task OrderNowConfirmFailed02_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderNowConfirmFailed02);
        }

        private async Task OrderNowConfirmApply_Entry()
        {
            var now = DateTime.Now;
            this.date = now.ToString("d");
            this.time = now.ToString("t");
        }

        private async Task OrderTodayConfirm_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderTodayConfirm01);
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task OrderTodayConfirmFailed01_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderTodayConfirmFailed01);
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task OrderTodayConfirmFailed02_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderTodayConfirmFailed02);
        }

        private async Task OrderTodayTime_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderTodayTime01);
            this.speechSubsystem.SetRecognitionMode("dates");
        }

        private async Task OrderTodayTimeFailed01_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderTodayTimeFailed01);
            this.speechSubsystem.SetRecognitionMode("dates");
        }

        private async Task OrderTodayTimeFailed02_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderTodayTimeFailed02);
        }

        private async Task OrderTodayTimeApply_Entry(String time)
        {
            var now = DateTime.Now;
            this.date = now.ToString("d");
            this.time = time;
        }

        private async Task OrderCustomDateTime_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderCustomDateTime01);
            this.speechSubsystem.SetRecognitionMode("dates");
        }

        private async Task OrderCustomDateTimeFailed01_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderCustomDateTimeFailed01);
            this.speechSubsystem.SetRecognitionMode("dates");
        }

        private async Task OrderCustomDateTimeFailed02_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderCustomDateTimeFailed02);
        }

        private async Task OrderCustomDateTimeApply_Entry(String datetime)
        {
            var now = DateTime.Now;
            this.date = datetime;
            this.time = "";
        }

        private async Task OrderCallerPhoneConfirm_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderCallerPhoneConfirm01);
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task OrderCallerPhoneConfirmFailed01_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderCallerPhoneConfirmFailed01);
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task OrderCallerPhoneConfirmFailed02_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderCallerPhoneConfirmFailed02);
        }

        private async Task OrderCallerPhoneConfirmApply_Entry()
        {
            this.phone = "CALLER";
        }

        private async Task OrderPhoneRequest_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderPhoneRequest01);
            this.speechSubsystem.SetRecognitionMode("numbers");
        }

        private async Task OrderPhoneRequestFailed01_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderPhoneRequestFailed01);
            this.speechSubsystem.SetRecognitionMode("numbers");
        }

        private async Task OrderPhoneRequestFailed02_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderPhoneRequestFailed02);
        }

        private async Task OrderPhoneRequestApply_Entry(String phone)
        {
            this.phone = phone;
        }

        private async Task OrderAdditionalInfoRequest_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderAdditionalInfoRequest01);
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task OrderAdditionalInfoRequestFailed01_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderAdditionalInfoRequestFailed01);
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task OrderAdditionalInfoRequestFailed02_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderAdditionalInfoRequestFailed02);
        }

        private async Task OrderAdditionalInfoNoneApply_Entry()
        {
            this.additionalInfo = "";
        }

        private async Task OrderAdditionalInfo_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderAdditionalInfo01);
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task OrderAdditionalInfoFailed01_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderAdditionalInfoFailed01);
            this.speechSubsystem.SetRecognitionMode("queries");
        }

        private async Task OrderAdditionalInfoFailed02_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderAdditionalInfoFailed02);
        }

        private async Task OrderAdditionalInfoApply_Entry(String additionalInfo)
        {
            this.additionalInfo = additionalInfo;
        }

        private async Task OrderCreated_Entry()
        {
            await this.speechSubsystem.SpeakAsync(OrderDialogResources.OrderCreated01);
        }

        private async Task Aborted_Entry()
        {
            this.speechSubsystem.StopCommunication();
        }

        private async Task Complete_Entry()
        {
            this.speechSubsystem.StopCommunication();
            await this.ordersService.CreateOrderAsync(new OrderInfo
            {
                AddressFromStreet = $"{this.addressFrom.StreetType} {this.addressFrom.StreetName}",
                AddressFromHouse = $"{this.addressFrom.StreetNumber}",
                AddressToStreet = $"{this.addressTo.StreetType} {this.addressTo.StreetName}",
                AddressToHouse = $"{this.addressTo.StreetNumber}",
                DateTime = $"{this.date} {this.time}".Trim(),
                Phone = this.phone,
                AdditionalInfo = this.additionalInfo
            });
        }

        private Boolean IsValidYes(String text)
        {
            return text.ToLower() == "да";
        }

        private Boolean IsValidNo(String text)
        {
            return text.ToLower() == "нет";
        }

        private Boolean IsValidAddress(String text)
        {
            return AddressParser.ParseAddress(text) != null;
        }

        private Boolean IsValidTime(String time)
        {
            return true;
        }

        private Boolean IsValidDateTime(String datetime)
        {
            return true;
        }

        private Boolean IsValidPhone(String phoneNumber)
        {
            return true;
        }

        private Boolean IsValidAdditionalInfo(String additionalInfo)
        {
            return true;
        }

        private enum State
        {
            Initial,
            Greeting,
            GreetingFailed01,
            GreetingFailed02,
            NotOrder,
            OrderFrom,
            OrderFromFailed01,
            OrderFromFailed02,
            OrderFromConfirm,
            OrderFromConfirmFailed01,
            OrderFromConfirmFailed02,
            OrderFromNotConfirmed,
            OrderTo,
            OrderToFailed01,
            OrderToFailed02,
            OrderToConfirm,
            OrderToConfirmFailed01,
            OrderToConfirmFailed02,
            OrderToNotConfirmed,
            OrderNowConfirm,
            OrderNowConfirmFailed01,
            OrderNowConfirmFailed02,
            OrderNowConfirmApply,
            OrderTodayConfirm,
            OrderTodayConfirmFailed01,
            OrderTodayConfirmFailed02,
            OrderTodayTime,
            OrderTodayTimeFailed01,
            OrderTodayTimeFailed02,
            OrderTodayTimeApply,
            OrderCustomDateTime,
            OrderCustomDateTimeFailed01,
            OrderCustomDateTimeFailed02,
            OrderCustomDateTimeApply,
            OrderCallerPhoneConfirm,
            OrderCallerPhoneConfirmFailed01,
            OrderCallerPhoneConfirmFailed02,
            OrderCallerPhoneConfirmApply,
            OrderPhoneRequest,
            OrderPhoneRequestFailed01,
            OrderPhoneRequestFailed02,
            OrderPhoneRequestApply,
            OrderAdditionalInfoRequest,
            OrderAdditionalInfoRequestFailed01,
            OrderAdditionalInfoRequestFailed02,
            OrderAdditionalInfoNoneApply,
            OrderAdditionalInfo,
            OrderAdditionalInfoFailed01,
            OrderAdditionalInfoFailed02,
            OrderAdditionalInfoApply,
            OrderCreated,
            Aborted,
            Complete
        }

        private enum Trigger
        {
            Proceed,
            ProcessResponse,
            RecognitionError
        }
    }
}
