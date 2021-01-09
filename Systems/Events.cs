/*
MIT License
Copyright (c) 2020 Unary Incorporated
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;

namespace Unary.L4D2_Randomizer.Systems
{
    public class Events
    {
        private Dictionary<string, List<Tuple<string, object>>> Subscribers;

        public Events()
        {
            Subscribers = new Dictionary<string, List<Tuple<string, object>>>();
        }

        public void Invoke(string Event, params object[] Args)
        {
            if (Subscribers.ContainsKey(Event))
            {
                foreach (var Subscriber in Subscribers[Event])
                {
                    var Method = Subscriber.Item2.GetType().GetMethod(Subscriber.Item1);
                    Method.Invoke(Subscriber.Item2, Args);
                }
            }
            else
            {
                Sys.Ref.Console.Error("Tried to invoke an event with no subscribers");
            }
        }

        public void Subscribe(string EventName, string MethodName, object Target)
        {
            if (!Subscribers.ContainsKey(EventName))
            {
                Subscribers[EventName] = new List<Tuple<string, object>>();
            }

            Subscribers[EventName].Add(new Tuple<string, object>(MethodName, Target));
        }
    }
}