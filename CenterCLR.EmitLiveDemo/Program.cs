using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CenterCLR.EmitLiveDemo
{
	class Program
	{
		private static Action GenerateAction()
		{
			var assemblyName = new AssemblyName("EmitLive");

			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule("EmitLiveModule");

			// public static class SampleClass { ... } のようなクラスを定義する
			var typeBuilder = moduleBuilder.DefineType(
				"SampleClass",
				TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.Class,
				typeof(object));

			//////////////////////////////////////////////////////////

			// public static void SampleMethod() { ... } のようなメソッドを定義する
			var methodBuilder = typeBuilder.DefineMethod(
				"SampleMethod",
				MethodAttributes.Public | MethodAttributes.Static,
				CallingConventions.Standard,
				typeof (void),
				Type.EmptyTypes);

			// Console.WriteLine(string) メソッドのメソッド定義を取得する
			var writeLineMethod = typeof(Console).GetMethod("WriteLine", new[] { typeof(string) });


			var ilGenerator = methodBuilder.GetILGenerator();
			ilGenerator.Emit(OpCodes.Ldstr, "Hello Center CLR!");
			ilGenerator.Emit(OpCodes.Call, writeLineMethod);
			ilGenerator.Emit(OpCodes.Ret);





			//////////////////////////////////////////////////////////

			// 使用可能な型として実体化する
			var type = typeBuilder.CreateType();

			// そこから使用可能なメソッド定義を取得する
			var method = type.GetMethod("SampleMethod");

			// メソッド定義が示すメソッドを呼び出し可能なデリゲートを生成する
			return (Action)Delegate.CreateDelegate(typeof(Action), method);
		}

		static void Main(string[] args)
		{
			// デリゲート生成
			var action = GenerateAction();

			// 実行
			action();
		}
	}
}
