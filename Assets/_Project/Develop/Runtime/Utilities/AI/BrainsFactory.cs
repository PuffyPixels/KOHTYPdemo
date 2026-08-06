using Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant;
using Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant.States;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using Assets._Project.Develop.Runtime.Gameplay.Shop;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Sound;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI
{
    public class BrainsFactory
    {
        private readonly DIContainer _container;
        private readonly Inventory _inventory;

        private readonly SoundsManager _soundsManager;

        public BrainsFactory(DIContainer container, Inventory inventory)
        {
            _container = container;
            _inventory = inventory;
            _soundsManager = _container.Resolve<SoundsManager>();
        }

        public StateMachineBrain CreateConsultantBrain(ConsultantSettings settings, ConsultantFacade consultant)
        {
            AIStateMachine stateMachine = CreateConsultantStateMachine(settings, consultant);
            StateMachineBrain brain = new StateMachineBrain(stateMachine);

            return brain;
        }

        private AIStateMachine CreateConsultantStateMachine(ConsultantSettings settings, ConsultantFacade consultant)
        {
            AIStateMachine stateMachine = new();

            AnabiosisState anabiosis = new(consultant, consultant.AnabiosisSounds, _soundsManager); // анабиос (не двигается)
            stateMachine.AddState(anabiosis);

            PatrolState patrol = new(consultant, consultant.PatrolSounds, _soundsManager); // патрулирование (патрулирует по точкам)
            stateMachine.AddState(patrol);

            AttentionState attention = new(consultant, consultant.AttentionSounds, _soundsManager); // внимание (затаился и всматривается)
            stateMachine.AddState(attention);

            InvestigateState investigate = new(consultant, consultant.InvestigateSounds, _soundsManager); // чутье (идёт в сторону игрока)
            stateMachine.AddState(investigate);

            ChaseState chase = new(consultant, consultant.ChaseSounds, _soundsManager); // обнаружение (идёт к игроку с увеличенной скоростью)
            stateMachine.AddState(chase);

            CaptureState capture = new(consultant, consultant.AnabiosisSounds, _soundsManager); // поимка (дошёл до игрока, отбирает все предметы)
            stateMachine.AddState(capture);


            ICondition anabiosis_patrol = new FuncCondition(() => _inventory.IsQuestItemFinded); // найден первый предмет
            stateMachine.AddTransition(anabiosis, patrol, anabiosis_patrol);

            ICondition patrol_attention = new FuncCondition(() => consultant.Target != null); // частичное обнаружение началось
            stateMachine.AddTransition(patrol, attention, patrol_attention);
            ICondition patrol_capture = new FuncCondition(() => patrol.IsPlayerCaptured); // поймали игрока
            stateMachine.AddTransition(patrol, capture, patrol_capture);

            ICondition attention_patrol = new FuncCondition(() => consultant.DetectionProgress <= 0f); // игрок потерян
            stateMachine.AddTransition(attention, patrol, attention_patrol);
            ICondition attention_investigate = new FuncCondition(() => consultant.DetectionProgress >= 0.5f); // частичное обнаружение началось
            stateMachine.AddTransition(attention, investigate, attention_investigate);
            ICondition attention_capture = new FuncCondition(() => attention.IsPlayerCaptured); // поймали игрока
            stateMachine.AddTransition(attention, capture, attention_capture);

            ICompositeCondition investigate_attention = new CompositeCondition()
                .Add(new FuncCondition(() => investigate.IsInvestigationComplete))
                .Add(new FuncCondition(() => consultant.DetectionProgress <= 0.5f)); // игрок потерян
            stateMachine.AddTransition(investigate, attention, investigate_attention);
            ICondition investigate_chase = new FuncCondition(() => consultant.DetectionProgress >= 1f); // полное обнаружение во время чутья
            stateMachine.AddTransition(investigate, chase, investigate_chase);
            ICondition investigate_capture = new FuncCondition(() => investigate.IsPlayerCaptured); // поймали игрока
            stateMachine.AddTransition(investigate, capture, investigate_capture);

            ICondition chase_investigate = new FuncCondition(() => consultant.DetectionProgress < 1f); // игрок потерян
            stateMachine.AddTransition(chase, investigate, chase_investigate);
            ICondition chase_capture = new FuncCondition(() => chase.IsPlayerCaptured); // поймали игрока
            stateMachine.AddTransition(chase, capture, chase_capture);

            ICondition capture_anabiosis = new FuncCondition(() => capture.IsCaptureComplete); // поимка завершена
            stateMachine.AddTransition(capture, anabiosis, capture_anabiosis);

            return stateMachine;
        }
    }
}
