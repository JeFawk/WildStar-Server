// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Arctium.Core.Logging;
using Arctium.Core.Network.Pipes;

namespace Arctium.Manager.Pipes
{
    class IPCPacketManager
    {
        static readonly Dictionary<byte, Tuple<MethodInfo, Type>> messageHandlers = new Dictionary<byte, Tuple<MethodInfo, Type>>();

        public static void DefineMessageHandler()
        {
            var currentAsm = Assembly.GetEntryAssembly();

            foreach (var type in currentAsm.GetTypes())
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    foreach (var msgAttr in methodInfo.GetCustomAttributes<IPCMessageAttribute>())
                        messageHandlers.TryAdd(msgAttr.Message, Tuple.Create(methodInfo, methodInfo.GetParameters()[0].ParameterType));
                }
            }
        }

        public static async Task CallHandler(byte ipcMessage, Stream ipcMessageData, IPCSession session)
        {
            Tuple<MethodInfo, Type> data;

            if (messageHandlers.TryGetValue(ipcMessage, out data))
            {
                var handlerObj = Activator.CreateInstance(data.Item2, ipcMessage, ipcMessageData) as PipePacketBase;

                await Task.Run(() => data.Item1.Invoke(null, new object[] { handlerObj, session }));
            }
            else
            {
                var msgName = Enum.GetName(typeof(IPCMessage), ipcMessage) ?? Enum.GetName(typeof(GlobalPipeMessage), ipcMessage);

                if (msgName == null)
                    Log.Message(LogTypes.Warning, $"Received unknown ipc message '0x{ipcMessage:X}'.");
                else
                    Log.Message(LogTypes.Warning, $"Handler for '{msgName} (0x{ipcMessage:X}) not implemented.");
            }
        }
    }
}
