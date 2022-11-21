// using Rebus.Activation;
// using Rebus.Config;
// using Rebus.Kafka;
// using Rebus.Routing.TypeBased;
// using shared_stuff;
//
// using var activator = new BuiltinHandlerActivator();
//
// Configure.With(activator)
//     .Transport(t => t.UseKafkaAsOneWayClient(ConstStrings.kafkaUrl))
//     // .Transport(t => t.UseKafka(ConstStrings.kafkaUrl,ConstStrings.topic,ConstStrings.topic))
//     .Routing(x => x.TypeBased())
//     .Logging(x=>x.Console())
//     .Start();
//
//
// foreach (int i in Enumerable.Range(0,10))
// {
//     
//     Console.WriteLine("sending stuff");
//     activator.Bus.Publish(new Step1Event
//     {
//         Counter =i 
//     });    
// }
// Console.WriteLine("Done");
