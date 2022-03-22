using OpenQuant.API;

public class MyStrategy : Strategy
{
   private Order order;
   private bool entry = true;
   
   public override void OnBar(Bar bar)
   {
      if (HasPosition)
         ClosePosition();

      if (entry)
       {
         order = LimitOrder(OrderSide.Buy, 100, bar.Close - 0.03);                     
         order.Send();
            
         AddTimer(Clock.Now.AddSeconds(20));

         entry = false;
      }
   }
   
   public override void OnTimer(DateTime signalTime)
   {   
      if (!order.IsDone)
         order.Cancel();

      entry = true;
   }
}
