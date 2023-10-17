﻿using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SDA.MANAGER.Manager
{
    public class ManagerContainer
    {
        public ManagerContainer(Type classTypeInvoke, string methodNameInvoke, object[] constructParameterInvoke, object[] methodParameterInvoke)
        {
            classType = classTypeInvoke;
            methodName = methodNameInvoke;
            ConstructParameters = constructParameterInvoke;
            MethodParameters = methodParameterInvoke;
        }

        private long maxProcessTimeInMilliseconds = 5000; //5 giay
        private Type classType;
        private string methodName;
        private object[] MethodParameters
        {
            get
            {
                try
                {
                    if (methodParameters != null && methodParameters.Length > 0 && String.IsNullOrEmpty(inputMethodDataLog))
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0, length = methodParameters.Length; i < length; i++)
                        {
                            sb.Append(Inventec.Common.Logging.LogUtil.TraceData((i + 1).ToString(), methodParameters[i]));
                        }
                        inputMethodDataLog = sb.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return methodParameters;
            }
            set
            {
                methodParameters = value;
                if (value != null && value.Length > 0)
                {
                    methodParameterTypes.Clear();
                    foreach (var item in value)
                    {
                        methodParameterTypes.Add(item.GetType());
                        var genericArgs = classType.GetGenericArguments();
                    }
                }
            }
        }
        private object[] ConstructParameters
        {
            set
            {
                constructParameters = value;
                if (value != null && value.Length > 0)
                {
                    constructParameterTypes.Clear();
                    foreach (var item in value)
                    {
                        constructParameterTypes.Add(item.GetType());
                    }
                }
            }
        }
        private string inputMethodDataLog;
        private object[] methodParameters;
        private object[] constructParameters;
        List<Type> methodParameterTypes = new List<Type>();
        List<Type> constructParameterTypes = new List<Type>();

        public T Run<T>()
        {
            T result = default(T);
            try
            {
                object classInstance = (constructParameterTypes != null && constructParameterTypes.Count > 0) ? Activator.CreateInstance(classType, constructParameters) : Activator.CreateInstance(classType, null);

                ///Coding convention lop Manager
                ///- Tuyet doi khong duoc chua 2 method trung ten
                ///- Constructor khong su dung kieu generic
                ///- Tham so dau tien cua constructor (neu co) phai la CommonParam
                ///- Tham so truyen vao ham khong su dung kieu generic
                ///- Chi su dung generic (neu can) cho kieu du lieu tra ve
                MethodInfo methodInfo = classType.GetMethod(methodName);
                if (methodInfo.IsGenericMethod)
                {
                    if (methodInfo.ReturnType.IsGenericParameter)
                    {
                        methodInfo = methodInfo.MakeGenericMethod(typeof(T));
                    }
                }

                var watch = System.Diagnostics.Stopwatch.StartNew();
                object originalResult = null;
                if (methodParameters != null && methodParameters.Length > 0)
                {
                    originalResult = methodInfo.Invoke(classInstance, MethodParameters);
                }
                else
                {
                    originalResult = methodInfo.Invoke(classInstance, null);
                }
                result = (T)System.Convert.ChangeType(originalResult, typeof(T));
                watch.Stop();

                TimeLog(watch);
                FailLog<T>(result, methodInfo);
                TroubleCheck();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = default(T);
            }
            return result;
        }

        private void TimeLog(System.Diagnostics.Stopwatch watch)
        {
            try
            {
                long time = watch.ElapsedMilliseconds;
                if (time > maxProcessTimeInMilliseconds)
                {
                    string className = classType.Name;
                    Inventec.Common.Logging.LogTime.Warn(new StringBuilder().Append(classType.Name).Append(".").Append(methodName).Append(".").Append(GetThreadId()).Append("___Thoi gian xu ly lau hon gioi han canh bao: ").Append(maxProcessTimeInMilliseconds).Append(" (milliseconds), la:").Append(time.ToString()).Append("(milliseconds).").ToString());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FailLog<T>(T result, MethodInfo methodInfo)
        {
            try
            {
                if (!Inventec.Core.Util.DecisionApiResult(result))
                {
                    Inventec.Common.Logging.LogSystem.Warn(new StringBuilder().Append(classType.Name).Append(".").Append(methodInfo.Name).Append(".").Append(GetThreadId()).Append("___Xu ly that bai___").Append("___Input:").Append(inputMethodDataLog).Append("___Output:").Append(Inventec.Common.Logging.LogUtil.TraceData("result", result)).ToString());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TroubleCheck()
        {
            try
            {
                if (constructParameters != null && constructParameters.Length > 0)
                {
                    CommonParam param = (CommonParam)constructParameters[0];
                    if (param.HasException || (param.BugCodes != null && param.BugCodes.Count > 0))
                    {
                        SDA.MANAGER.Base.TroubleCache.Add(GetThreadId() + (param.HasException ? "param.HasException." : "") + param.GetBugCode());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetThreadId()
        {
            try
            {
                return "___{ThreadId=" + System.Threading.Thread.CurrentThread.ManagedThreadId + "}___";
            }
            catch (Exception)
            {
                return "___{ThreadId=ManagerContainer.GetThreadId.HasException}___";
            }
        }
    }
}
