using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GasxherGIS.Application.Internal;

namespace GasxherGIS.Application
{
   
    public class ApplicationFlowBuilder : IApplicationControlFlow
    {
        private List<Type> steps = new List<Type>();
        public List<string> stepsName { get; private set; } = new List<string>();

        private readonly IServiceProvider _providers;

        private readonly ILogger<ApplicationFlowBuilder> _logger;

        public IApplicationConsole[] stepsConsole { get; private set; }

        public bool avoidExitOnStepException { get; private set; } = false;


        public ApplicationFlowBuilder(IServiceProvider providers, ILogger<ApplicationFlowBuilder> logger)
        {
            _providers = providers;
            _logger = logger;
        }

        /// <summary>
        /// Default name by class name
        /// </summary>
        /// <param name="stepType"></param>
        /// <returns></returns>
        public IApplicationControlFlow Step(Type stepType)
        {
            if (steps == null)
            {
                throw new ArgumentNullException(nameof(stepType));
            }

            stepsName.Add(stepType.Name);
            steps.Add(stepType);

            return this;
        }


        /// <summary>
        /// Custom step name
        /// </summary>
        /// <param name="stepName"></param>
        /// <param name="stepType"></param>
        /// <returns></returns>
        public IApplicationControlFlow Step(string stepName, Type stepType)
        {
            if (steps == null)
            {
                throw new ArgumentNullException(nameof(stepType));
            }

            stepsName.Add(stepName);
            steps.Add(stepType);

            return this;
        }


        public IApplicationControlFlow Step<TStep>() => this.Step(typeof(TStep));


        public IApplicationControlFlow Step<TStep>(string stepName) => this.Step(stepName, typeof(TStep));


        public IApplicationControlFlow AvoidExitOnStepException(bool exit)
        {
            this.avoidExitOnStepException = exit;

            return this;
        }


        public void Run()
        {
            Stopwatch sw = new Stopwatch();

            //=>Assign arrays
            var exceptions = new List<bool>();
            stepsConsole = new IApplicationConsole[steps.Count];

            int currentStep = 1;

            foreach (var step in steps)
            {
                bool isException = false;

                //=>Begin
                sw.Start();
                _logger.LogInformation($"==>Running step: {currentStep}/{steps.Count} : {stepsName[currentStep - 1]}...");

                //=>New instance
                try
                {
                    var console = (IApplicationConsole)ActivatorUtilities.CreateInstance(_providers, step);
                    stepsConsole[currentStep - 1] = console;
                    console.Main();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Step: {currentStep}/{steps.Count} Error: {ex.ToString()}");
                    isException = true;
                }

                //=>End begin
                sw.Stop();
                _logger.LogInformation($"Timer: {sw.Elapsed.TotalSeconds.ToString("0.###")} sec.");

                if (!isException)
                {
                    _logger.LogInformation($"Run complete");
                }
                else
                {
                    _logger.LogInformation($"Run fail");
                }

                exceptions.Add(isException == true);
                currentStep++;

                if (isException && avoidExitOnStepException)
                {
                    break;
                }
            }

            _logger.LogInformation($"Summarize...");
            _logger.LogInformation($"Complete step {exceptions.Where(w => !w).ToList().Count}/{steps.Count}");
        }
    }
}
