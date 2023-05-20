using System.Collections;

namespace Combat.Requests.Details {
    public class RequestEndTurn : CombatRequest {
        private int DiscardCardCnt => Causer.Cards.Count - Causer.State.MaxCardCnt;
        
        public override bool CanEnqueue() {
            return Require(
                Causer != null,
                "无效的结束回合请求"
            );
        }

        public override IEnumerator Execute() {
            if (DiscardCardCnt > 0) {
                Causer.Controller.isDiscardStage = true;
            }
            else {
                Causer.Controller.isDiscardStage = false;
                AddPost(() => Judge.NextTurn());
            }
            return null;
        }

        public override string Description() {
            if (DiscardCardCnt > 0) {
                return $"{Causer.name}结束回合：卡牌数超过上限，需要丢弃{DiscardCardCnt}张牌。";
            }
            return $"{Causer.name}结束回合";
        }
    }
}