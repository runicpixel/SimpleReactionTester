using SimpleReactionMachine;

namespace Controller
{
    public class SimpleReactionController : IController
    {
        //declaring timer constants
        private const int min_wait_time = 100;
        private const int max_wait_time = 250;
        private const int game_time = 200;
        private const int gameover_time = 300;
        private const double ticks_per_sec = 100.0;

        //istance variables of class
        private State _state;

        //properties of behaviour
        private IGui Gui{get; set;}
        private IRandom Rand{get; set;}
        private int Ticks{get; set;}
        public void Connect(IGui gui,IRandom random)
        {
            Gui = gui;
            Rand = random;
            Init();
        }
        public void Init()
        {
            _state = new StartState(this);
        }
        public void CoinInserted()
        {
            _state.CoinInserted();
        }
        public void GoStopPressed()
        {
            _state.GoStopPressed();
        }
        public void Tick()
        {
            _state.Tick();
        }
        private void SetState(State state)
        {
            this._state = state;
        }

        //abstract class state as in this program we will be defining and then calling different stages of state but not the state itself
        private abstract class State
        {
            protected SimpleReactionController _controller;
            public State(SimpleReactionController controller)
            {
                _controller = controller;
            }

            public abstract void CoinInserted();
            public abstract void GoStopPressed();
            public abstract void Tick();
        }

        // class of Starting state of the game
        private class StartState : State
        {
            public StartState(SimpleReactionController controller)
            :base(controller)
            {
                _controller.Gui.SetDisplay("Insert Coin");
            }

            //setting state to next state is coin insert pressed
            public override void CoinInserted()
            {
                _controller.SetState(new State2(_controller));
            }
            public override void GoStopPressed(){}
            public override void Tick(){}
        }

        // class of ready state
        private class State2 : State
        {
            public State2(SimpleReactionController controller)
            : base(controller)
            {
                _controller.Gui.SetDisplay("Press Go!");
            }
            public override void CoinInserted(){}
            public override void GoStopPressed()
            {
                _controller.SetState(new RetryState(_controller));
            }
            public override void Tick(){}
        }
        private class RetryState : State
        {
            public RetryState(SimpleReactionController controller)
            :base(controller)
            {
                _controller.Gui.SetDisplay("Wait or Insert Coin");
                
                _controller.Ticks = 0;
            }
            public override void CoinInserted()
            {
                _controller.SetState(new State2(_controller));
            }
            public override void GoStopPressed()
            {
            }
            public override void Tick()
            {
                _controller.Ticks++;
                if(_controller.Ticks == gameover_time)
                {
                    _controller.SetState(new WaitState(_controller));
                }
            }
        }

        // class of wait state here for sometime then proceed to next stage
        private class WaitState : State
        {
            private int _waitTime;
            public WaitState(SimpleReactionController controller)
            : base(controller)
            {
                _controller.Gui.SetDisplay("Wait...");
                _controller.Ticks = 0;
                _waitTime = _controller.Rand.GetRandom(min_wait_time,max_wait_time);
            }
            public override void CoinInserted(){}
            public override void GoStopPressed()
            {
                _controller.SetState(new StartState(_controller));
            }
            public override void Tick()
            {
                _controller.Ticks++;
                if(_controller.Ticks == _waitTime)
                {
                    _controller.SetState(new State4(_controller));
                }
            }
        }
        private class State4 : State
        {
            public State4(SimpleReactionController controller)
            : base(controller)
            {
                _controller.Gui.SetDisplay("0.00");
                _controller.Ticks = 0;
            }
            public override void CoinInserted(){}
            public override void GoStopPressed()
            {
                _controller.SetState(new EndState(_controller));
            }
            public override void Tick()
            {
                _controller.Ticks++;
                _controller.Gui.SetDisplay((_controller.Ticks/ticks_per_sec).ToString("0.00"));
                if(_controller.Ticks == game_time)
                    _controller.SetState(new EndState(_controller));
            }
        }
        private class EndState:State
        {
            public EndState(SimpleReactionController controller)
            : base(controller)
            {
                _controller.Ticks = 0;
            }
            public override void CoinInserted(){}
            public override void GoStopPressed()
            {
                _controller.SetState(new StartState(_controller));
            }
            public override void Tick()
            {
                _controller.Ticks++;
                if(_controller.Ticks == gameover_time)
                {
                    _controller.SetState(new StartState(_controller));
                }
            }
        }
    }
}
