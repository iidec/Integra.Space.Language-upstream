using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Globalization;
using System.Reflection;
using System.Linq;
using Integra.Space.Compiler;
using Integra.Space.Language;
using Integra.Space.Database;
using System.Reflection.Emit;
using Ninject;
using System.Reactive.Concurrency;

namespace Integra.Space.LanguageUnitTests.Constants
{
    [TestClass]
    public class DateTimeFunctionNodeTests
    {
        private CodeGeneratorConfiguration GetCodeGeneratorConfig(DefaultSchedulerFactory dsf)
        {
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            bool isTestMode = true;
            Login login = new SpaceDbContext().Logins.First();
            StandardKernel kernel = new StandardKernel();
            kernel.Bind<ISourceTypeFactory>().ToConstructor(x => new SourceTypeFactory());
            CodeGeneratorConfiguration config = new CodeGeneratorConfiguration(
                login,
                dsf,
                AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Test"), AssemblyBuilderAccess.RunAndSave),
                kernel,
                printLog: printLog,
                debugMode: debugMode,
                measureElapsedTime: measureElapsedTime,
                isTestMode: isTestMode
                );

            return config;
        }

        /// <summary>
        /// Este método no se usa en todas las pruebas de esta clase.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T Process<T>(string eql, DefaultSchedulerFactory dsf)
        {
            CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);

            FakePipeline fp = new FakePipeline();
            Assembly assembly = fp.ProcessWithExpressionParser(context, eql, dsf);

            Type[] types = assembly.GetTypes();
            Type queryInfo = assembly.GetTypes().First(x => x.GetInterface("IQueryInformation") == typeof(IQueryInformation));
            IQueryInformation queryInfoObject = (IQueryInformation)Activator.CreateInstance(queryInfo);
            Type queryType = queryInfoObject.GetQueryType();
            object queryObject = Activator.CreateInstance(queryType);
            MethodInfo result = queryObject.GetType().GetMethod("MainFunction");

            return (T)result.Invoke(queryObject, new object[] { dsf.TestScheduler });
        }
        /// <summary>
        /// Obtiene el segundo de la cadena especificada
        /// </summary>
        [TestMethod]
        public void YearFunction()
        {
            ExpressionParser parser = new ExpressionParser("year('02/01/2014 10:11:12 am')");
            PlanNode plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            CodeGeneratorConfiguration config = this.GetCodeGeneratorConfig(dsf);
            CodeGenerator te = new CodeGenerator(config);
            Func<IScheduler, int?> result = (Func<IScheduler, int?>)te.CompileDelegate(plan);

            Assert.AreEqual<int?>(2014, result(dsf.TestScheduler), "El plan obtenido difiere del plan esperado.");
        }

        /// <summary>
        /// Obtiene el segundo de la cadena especificada
        /// </summary>
        [TestMethod]
        public void MonthFunction()
        {
            string eql = "month('02/01/2014 10:11:12 am')";
            ExpressionParser parser = new ExpressionParser(eql);
            PlanNode plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            Assert.AreEqual<int?>(1, this.Process<int?>(eql, dsf), "El plan obtenido difiere del plan esperado.");
        }

        /// <summary>
        /// Obtiene el día de la cadena especificada
        /// </summary>
        [TestMethod]
        public void DayFunction1()
        {
            string eql = "day('02/01/2014 10:11:12 am')";
            ExpressionParser parser = new ExpressionParser(eql);
            PlanNode plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            Assert.AreEqual<int?>(2, this.Process<int?>(eql, dsf), "El plan obtenido difiere del plan esperado.");
        }

        /// <summary>
        /// Obtiene el día de la cadena especificada
        /// </summary>
        [TestMethod]
        public void DayFunction2()
        {
            string eql = "day('1.02:00:30')";
            ExpressionParser parser = new ExpressionParser(eql);
            PlanNode plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            Assert.AreEqual<int?>(1, this.Process<int?>(eql, dsf), "El plan obtenido difiere del plan esperado.");
        }

        /// <summary>
        /// Obtiene la hora de la cadena especificada
        /// </summary>
        [TestMethod]
        public void HourFunction1()
        {
            string eql = "hour('10:11:12 am')";
            ExpressionParser parser = new ExpressionParser("hour('01/01/2014 10:11:12 am')");
            PlanNode plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            Assert.AreEqual<int?>(10, this.Process<int?>(eql, dsf), "El plan obtenido difiere del plan esperado.");
        }

        /// <summary>
        /// Obtiene la hora de la cadena especificada
        /// </summary>
        [TestMethod]
        public void HourFunction2()
        {
            string eql = "hour('10:11:12 am')";
            ExpressionParser parser = new ExpressionParser(eql);
            PlanNode plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            Assert.AreEqual<int?>(10, this.Process<int?>(eql, dsf), "El plan obtenido difiere del plan esperado.");
        }

        /// <summary>
        /// Obtiene la hora de la cadena especificada
        /// </summary>
        [TestMethod]
        public void HourFunction3()
        {
            string eql = "hour('1.02:00:30')";
            ExpressionParser parser = new ExpressionParser(eql);
            PlanNode plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            Assert.AreEqual<int?>(2, this.Process<int?>(eql, dsf), "El plan obtenido difiere del plan esperado.");
        }

        /// <summary>
        /// Obtiene el minuto de la cadena especificada
        /// </summary>
        [TestMethod]
        public void MinuteFunction1()
        {
            string eql = "minute('01/01/2014 10:11:12 am')";
            ExpressionParser parser = new ExpressionParser(eql);
            PlanNode plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            Assert.AreEqual<int?>(11, this.Process<int?>(eql, dsf), "El plan obtenido difiere del plan esperado.");
        }

        /// <summary>
        /// Obtiene el minuto de la cadena especificada
        /// </summary>
        [TestMethod]
        public void MinuteFunction2()
        {
            string eql = "minute('1.02:10:30')";
            ExpressionParser parser = new ExpressionParser(eql);
            PlanNode plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            Assert.AreEqual<int?>(10, this.Process<int?>(eql, dsf), "El plan obtenido difiere del plan esperado.");
        }

        /// <summary>
        /// Obtiene el segundo de la cadena especificada
        /// </summary>
        [TestMethod]
        public void SecondFunction1()
        {
            string eql = "second('01/01/2014 10:11:12 am')";
            ExpressionParser parser = new ExpressionParser(eql);
            PlanNode plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            Assert.AreEqual<int?>(12, this.Process<int?>(eql, dsf), "El plan obtenido difiere del plan esperado.");
        }

        /// <summary>
        /// Obtiene el segundo de la cadena especificada
        /// </summary>
        [TestMethod]
        public void SecondFunction2()
        {
            string eql = "second('10:11:12 am')";
            ExpressionParser parser = new ExpressionParser(eql);
            PlanNode plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            Assert.AreEqual<int?>(12, this.Process<int?>(eql, dsf), "El plan obtenido difiere del plan esperado.");
        }

        /// <summary>
        /// Obtiene el segundo de la cadena especificada
        /// </summary>
        [TestMethod]
        public void SecondFunction3()
        {
            string eql = "second('1.02:10:30')";
            ExpressionParser parser = new ExpressionParser(eql);
            PlanNode plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            Assert.AreEqual<int?>(30, this.Process<int?>(eql, dsf), "El plan obtenido difiere del plan esperado.");
        }

        /// <summary>
        /// Obtiene el segundo de la cadena especificada
        /// </summary>
        [TestMethod]
        public void MillisecondFunction1()
        {
            string eql = "millisecond('01/01/2014 10:11:12.1 am')";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            Assert.AreEqual<int?>(100, this.Process<int?>(eql, dsf), "El plan obtenido difiere del plan esperado.");
        }

        /// <summary>
        /// Obtiene el segundo de la cadena especificada
        /// </summary>
        [TestMethod]
        public void MillisecondFunction2()
        {
            string eql = "millisecond('10:11:12.1 am')";
            ExpressionParser parser = new ExpressionParser(eql);
            PlanNode plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            Assert.AreEqual<int?>(100, this.Process<int?>(eql, dsf), "El plan obtenido difiere del plan esperado.");
        }

        /// <summary>
        /// Obtiene el segundo de la cadena especificada
        /// </summary>
        [TestMethod]
        public void MillisecondFunction3()
        {
            string eql = "millisecond('1.02:10:30.6')";
            ExpressionParser parser = new ExpressionParser(eql);
            PlanNode plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            Assert.AreEqual<int?>(600, this.Process<int?>(eql, dsf), "El plan obtenido difiere del plan esperado.");
        }
    }
}
