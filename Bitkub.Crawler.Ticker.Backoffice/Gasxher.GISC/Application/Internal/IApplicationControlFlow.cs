using System;

namespace GasxherGIS.Application.Internal
{
    public interface IApplicationControlFlow
    {
        /// <summary>
        /// ควบคุม Step การทำงาน แบบ Generic Type
        /// </summary>
        /// <typeparam name="TStep"></typeparam>
        /// <returns></returns>
        IApplicationControlFlow Step<TStep>();


        /// <summary>
        /// ควบคุม Step การทำงาน แบบ Generic Type และกำหนดชื่อ Step ได้
        /// </summary>
        /// <typeparam name="TStep"></typeparam>
        /// <returns></returns>
        IApplicationControlFlow Step<TStep>(string stepName);

        /// <summary>
        /// ควบคุม Step การทำงาน แบบ Type
        /// </summary>
        /// <param name="stepType"></param>
        /// <returns></returns>
        IApplicationControlFlow Step(Type stepType);


        /// <summary>
        /// ควบคุม Step การทำงาน แบบ Type และกำหนดชื่อ Step ได้
        /// </summary>
        /// <param name="stepName"></param>
        /// <param name="stepType"></param>
        /// <returns></returns>
        IApplicationControlFlow Step(string stepName, Type stepType);

        /// <summary>
        /// ควบคุมการ exit เมื่อเกิด Error
        /// default: true
        /// </summary>
        /// <param name="exit"></param>
        /// <returns></returns>
        IApplicationControlFlow AvoidExitOnStepException(bool exit);


        void Run();
    }

}
