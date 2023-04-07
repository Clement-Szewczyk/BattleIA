
            Console.WriteLine("localenergy = " + localenergy);
            
            if (localenergy == 1 && murnord == 0 ){
                localenergy = 0;
                Console.WriteLine(" direction nord");
                return BotHelper.ActionMove(MoveDirection.North);
                
            } 
            if (localenergy ==1 && murnord == 1){

               if (murest == 0){
                   localenergy = 0;
                   Console.WriteLine("direction est");
                   return BotHelper.ActionMove(MoveDirection.West);
               }
               if (murouest == 0){
                   localenergy = 0;
                   Console.WriteLine("direction ouest");
                   return BotHelper.ActionMove(MoveDirection.East);
               }
               else{
                    return BotHelper.ActionMove(MoveDirection.South);
               }
            }
            if (localenergy == 2 && mursud == 0 ){
                localenergy = 0;
                Console.WriteLine("direction sud");
                return BotHelper.ActionMove(MoveDirection.South);
            }
            if (localenergy == 2 && mursud == 1){
                if (murest == 0){
                     localenergy = 0;
                     Console.WriteLine("direction est");
                     return BotHelper.ActionMove(MoveDirection.West);
                }
                if (murouest == 0){
                     localenergy = 0;
                     Console.WriteLine("direction ouest");
                     return BotHelper.ActionMove(MoveDirection.East);
                }
                else{
                    return BotHelper.ActionMove(MoveDirection.North);
                }
            }
            if (localenergy == 4 && murouest == 0 ){
                localenergy = 0;
                Console.WriteLine(" direction ouest");
                return BotHelper.ActionMove(MoveDirection.East);
            }
            if (localenergy == 4 && murouest == 1){
                if (mursud == 0){
                     localenergy = 0;
                     Console.WriteLine(" direction sud");
                     return BotHelper.ActionMove(MoveDirection.South);
                }
                if (murnord == 0){
                     localenergy = 0;
                     Console.WriteLine("direction nord");
                     return BotHelper.ActionMove(MoveDirection.North);
                }
                else{
                      return BotHelper.ActionMove(MoveDirection.West);
                }
            }
            if (localenergy == 3 && murest == 0 ){
                localenergy = 0;
                Console.WriteLine("direction est");
                
                return BotHelper.ActionMove(MoveDirection.West);
            }
            if (localenergy == 3 && murest == 1){
                if (mursud == 0){
                     localenergy = 0;
                     Console.WriteLine("direction sud");
                     return BotHelper.ActionMove(MoveDirection.South);
                }
                if (murnord == 0){
                     localenergy = 0;
                     Console.WriteLine("direction nord");
                     return BotHelper.ActionMove(MoveDirection.North);
                }
                else{
                    return BotHelper.ActionMove(MoveDirection.East);
                }
            }
            if (murnord == 1){
                if (murest == 0){
                    return BotHelper.ActionMove(MoveDirection.West);
                }
                if (murouest == 0){
                    return BotHelper.ActionMove(MoveDirection.East);
                }
                if (murest == 1 || murouest == 1){
                    return BotHelper.ActionMove(MoveDirection.South);
                }
                else{
                    return BotHelper.ActionMove(MoveDirection.South);
                }
            }
            if (mursud == 1){
                if (murest == 0){
                    return BotHelper.ActionMove(MoveDirection.West);
                }
                if (murouest == 0){
                    return BotHelper.ActionMove(MoveDirection.East);
                }
                if (murest == 1 || murouest == 1){
                    return BotHelper.ActionMove(MoveDirection.North);
                }
                else{
                    return BotHelper.ActionMove(MoveDirection.North);
                }
            }
            if (murest == 1){
                if (mursud == 0){
                    return BotHelper.ActionMove(MoveDirection.South);
                }
                if (murnord == 0){
                    return BotHelper.ActionMove(MoveDirection.North);
                }
                else{
                    return BotHelper.ActionMove(MoveDirection.East);
                }
            }
            if (murouest == 1){
                if (mursud == 0){
                    return BotHelper.ActionMove(MoveDirection.South);
                }
                if (murnord == 0){
                    return BotHelper.ActionMove(MoveDirection.North);
                }
                else{
                    return BotHelper.ActionMove(MoveDirection.West);
                }
            }
            else{
               return BotHelper.ActionMove((MoveDirection)rnd.Next(1, 5));
            
            }