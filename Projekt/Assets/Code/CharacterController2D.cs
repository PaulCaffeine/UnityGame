using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour 
{
    // Skin wewnatrz obiektu.
    private const float SkinWidth = .02f;
    // Liczba promieni wykrywajacych kolizje
    // wychodzacych z obiektu.
    private const int TotalHorizontalRays = 8;
    private const int TotalVerticalRays = 4;

    // Wartosc uzywana do obliczania ruchu po pochylych powierzchniach.
    private static readonly float SlopeLimitTangent = Mathf.Tan(75f * Mathf.Deg2Rad);

    // Layer mask uzywany do wykrywania kolizji. 
    // Obiekty na tej warstwie uczestnicza w kolizji.
    public LayerMask PlatformMask;
    // Domyslne parametry kontrolera.
    public ControllerParameters2D DefaultParameters;

    // Stan gracza, informujacy o kolizjach w jakich bierze udzial.
    public ControllerState2D State { get; private set; }
    // Predkosc gracza.
    public Vector2 Velocity { get { return _velocity; } }
    // Zmienna typu bool, mowiaca czy nalezy obsluzyc kolizje.
    public bool HandleCollisions { get; set; }
    // Zwraca nadpisane lub w przypadku braku nadpisania,
    // domyslne parametry kontrolera.
    public ControllerParameters2D Parameters { get { return _overrideParameters ?? DefaultParameters;  } }
    public GameObject StandingOn { get; private set; }
    // Predkosc platformy, na ktorej stoi gracz.
    public Vector3 PlatformVelocity { get; private set; }

    // Sprawdza czy mozna wykonac skok zaleznie od ustawionej wartosci JumpBehavior.
    public bool CanJump 
    { 
        get 
        {
            if (Parameters.JumpRestrictions == ControllerParameters2D.JumpBehavior.CanJumpAnywhere)
                return _jumpIn <= 0;

            if (Parameters.JumpRestrictions == ControllerParameters2D.JumpBehavior.CanJumpOnGround)
                return State.IsGrounded;

            return false;
        } 
    }

    private Vector2 _velocity;
    private Transform _transform;
    private Vector3 _localScale;
    private BoxCollider2D _boxCollider;
    private ControllerParameters2D _overrideParameters;
    private float _jumpIn;
    private GameObject _lastStandingOn;

    private Vector3
        _activeGlobalPlatformPoint,
        _activeLocalPlatformPoint;

    private Vector3
        _raycastTopLeft,
        _raycastBottomRight,
        _raycastBottomLeft;

    // Odleglosci pomiedzy promieniami wykrywajacymi kolizje.
    private float 
        _verticalDistanceBetweenRays,
        _horizontalDistanceBetweenRays;

    // Inicjalizacja i ustawienie poczatkowych wartosci parametrow.
    public void Awake()
    {
        HandleCollisions = true;
        State = new ControllerState2D();
        _transform = transform;
        _localScale = transform.localScale;
        _boxCollider = GetComponent<BoxCollider2D>();

        // Obliczenie rozmiarow boxu, ktory moze uczestniczyc w kolizjach
        // z innymi brylami sztywnymi. Od wartosci odejmowane sa skiny, z ktorych
        // wychodza promienie wykrywajace kolizje (skin to waska ramka wokol obiektu,
        // istniejaca po wewnetrznej stronie obiektu, sluzaca do wykrywania kolizji).
        // Nastepnie obliczane sa odstepy pomiedzy promieniami wykrywajacymi kolizje.
        var colliderWidth = _boxCollider.size.x * Mathf.Abs(transform.localScale.x) - (2 * SkinWidth);
        _horizontalDistanceBetweenRays = colliderWidth / (TotalVerticalRays - 1);

        var colliderHeight = _boxCollider.size.y * Mathf.Abs(transform.localScale.y) - (2 * SkinWidth);
        _verticalDistanceBetweenRays = colliderHeight / (TotalHorizontalRays - 1);
    }

    // Dodanie dodatkowej predkosci.
    public void AddForce(Vector2 force)
    {
        _velocity += force;
    }

    // Ustawienie predkosci.
    public void SetForce(Vector2 force)
    {
        _velocity = force;
    }

    // Ustawienie predkosci w poziomie.
    public void SetHorizontalForce(float x)
    {
        _velocity.x = x;
    }

    // Ustawienie predkosci w pionie.
    public void SetVerticalForce(float y)
    {
        _velocity.y = y;
    }

    // Wykonanie skoku.
    public void Jump()
    {
        AddForce(new Vector2(0, Parameters.JumpMagnitude));
        _jumpIn = Parameters.JumpFrequency;
    }

    // Wykonywane po wywolaniu aktualizacji obiektow.
    // Uwzglednienie wplywu czasu na czestotliwosc skokow i ruch,
    // wplyw grawitacji na ruch w pionie.
    public void LateUpdate()
    {
        _jumpIn -= Time.deltaTime;
        _velocity.y += Parameters.Gravity * Time.deltaTime;
        Move(Velocity * Time.deltaTime);
    }

    // Metoda odpowiedzialna za obsluge ruchu.
    private void Move(Vector2 deltaMovement)
    {
        // Zapamietujemy czy gracz znajduje sie na podlozu, a
        // nastepnie resetujemy stan.
        var wasGrounded = State.IsCollidingBelow;
        State.Reset();

        if (HandleCollisions)
        {
            HandlePlatforms();
            CalculateRayOrigins();

            // Ruch w pionie to tez ruch spowodowany grawitacja, nastepnie sprawdzamy czy
            // jestesmy na ziemi i sprawdzamy czy nalezy obsluzyc pionowy aspekt
            // ruchu po pochylej powierzchni.
            if (deltaMovement.y < 0 && wasGrounded)
                HandleVerticalSlope(ref deltaMovement);

            // Jesli wykryto ruch w lewo lub prawo,
            // obslugiwany jest ruch w poziomie.
            if (Mathf.Abs(deltaMovement.x) > .001f)
                MoveHorizontally(ref deltaMovement);

            // Ruch w pionie obslugiwany jest zawsze ze wzgledu na grawitacje.
            MoveVertically(ref deltaMovement);

            // Wywolywana metoda obslugujaca zderzenia z platfroma
            // w poziomie, dla sytuacji gdy gracz porusza sie
            // w prawo lub w lewo.
            CorrectHorizontalPlacement(ref deltaMovement, true);
            CorrectHorizontalPlacement(ref deltaMovement, false);
        }

        // Wartosc wykonanego ruchu jest przekazywana do Unity.
        _transform.Translate(deltaMovement, Space.World);

        // Predkosc aktualizowana jest o uplyw czasu w grze.
        if (Time.deltaTime > 0)
            _velocity = deltaMovement / Time.deltaTime;

        // Ograniczenie wartosci predkosci do obecnej lub 
        // ustalonej jako maksymalna.
        _velocity.x = Mathf.Min(_velocity.x, Parameters.MaxVelocity.x);
        _velocity.y = Mathf.Min(_velocity.y, Parameters.MaxVelocity.y);

        // Jesli poruszamy sie w gore po pochylej powierzchni,
        // predkosc w pionie przyjmuje wartosc 0.
        if (State.IsMovingUpSlope)
            _velocity.y = 0;

        // Jesli gracz stoi na jakims obiekcie.
        if (StandingOn != null)
        {
            // Pozycja gracza.
            _activeGlobalPlatformPoint = transform.position;
            // Pozycja gracza w relacji do platformy, na ktorej stoi.
            _activeLocalPlatformPoint = StandingOn.transform.InverseTransformPoint(transform.position);

            Debug.DrawLine(transform.position, _activeGlobalPlatformPoint);
            Debug.DrawLine(transform.position, _activeLocalPlatformPoint);

            // Nie stoimy juz na obiekcie, na ktorym
            // stalismy w ostatniej klatce animacji gry.
            if (_lastStandingOn != StandingOn)
            {
                // Jesli istnieje obiekt, na ktorym stalismy w ostaniej klatce,
                // wysylany jest do niego komunikat o wyjściu.
                if (_lastStandingOn != null)
                    _lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);

                // Do obiektu, na ktorym obecnie stoimy 
                // wysylany jest komunikat o wejsciu.
                StandingOn.SendMessage("ControllerEnter2D", this, SendMessageOptions.DontRequireReceiver);
                _lastStandingOn = StandingOn;
            }
            // Jesli istnieje obiekt, na ktorym obecnie stoimy i
            // stalismy na nim w ostatniej klatce, wysylany jest
            // do niego komunikat o pozostaniu.
            else if (StandingOn != null)
            {
                StandingOn.SendMessage("ControllerStay2D", this, SendMessageOptions.DontRequireReceiver);
            }
        }
        // Jesli gracz obecnie nie stoi na zadnym obiekcie,
        // ale stal na obiekcie w ostaniej klatce, wysylany jest 
        // do niego komunikat o pozostaniu.
        else if (_lastStandingOn != null)
        {
            _lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);
            _lastStandingOn = null;
        }
    }

    // Metoda ta dba o to, aby gracz stojacy na platformie 
    // nie zsuwal sie z niej, ale poruszal sie razem z nia.
    private void HandlePlatforms()
    {
        // Jesli w ostatniej klatce gry stalismy na jakims obiekcie.
        if (StandingOn != null)
        {
            // _activeGlobalPlatformPoint - gdzie platforma byla.
            // newGlobalPlatformPoint - gdzie platforma jest.
            var newGlobalPlatformPoint = StandingOn.transform.TransformPoint(_activeLocalPlatformPoint);
            // Roznica miedzy tym gdzie platforma jest, a tym gdzie byla,
            // co daje nam przebyty dystans.
            var moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;

            // Jesli przebyty dyatans jest rozny od zera,
            // informacja o przebytej odleglosci jest przekazywana do Unity.
            if (moveDistance != Vector3.zero)
                // Gracz jest urzymywany na platformie.
                transform.Translate(moveDistance, Space.World);

            // Predkosc platformy to przebyty dystans, dzielony
            // przez czas, ktory uplynal w grze.
            PlatformVelocity = (newGlobalPlatformPoint - _activeGlobalPlatformPoint) / Time.deltaTime;
        }
        // Jesli gracz nie stoi na platformie, to wartosc zwiazana z tym jaka
        // predkosc ma platforma, na ktorej (tu teoretycznie) stoi gracz
        // jest rowna zeru.
        else
        {
            PlatformVelocity = Vector3.zero;
        }

        // Wartosc dotyczaca tego czy stoimy na obiekcie 
        // jest ustawiana na null.
        StandingOn = null;
    }

    // Obsluga sytuacji, w ktorej gracz wskakuje w boczna sciane platformy.
    // Metoda ta zapobiega przechodzeniu w takiej sytuacji przez platformę.
    // Nastepuje pozadana kolizja w pozimie miedzy graczem, a platforma.
    private void CorrectHorizontalPlacement(ref Vector2 deltaMovement, bool isRight)
    {
        // halfWidth - rozmiar boxu kolizji, podzielony na pol.
        var halfWidth = (_boxCollider.size.x * _localScale.x) / 2f;
        // Ustalenie punktu startowego promieni, zalezne od zwrotu gracza.
        var rayOrigin = isRight ? _raycastBottomRight : _raycastBottomLeft;

        // Jesli poruszamy sie w prawo, punkt startowy promieni
        // bedzie znajdowal sie w polowie boxu kolizji.
        // Operacja odjecia polowy boxu kolizji od
        // pozycji _raycastBottomRight, spowoduje ze
        // znajdziemy sie o skin na lewo od centrum boxa,
        // dlatego nastepnie wykonywana jest korekta o te wartosc.
        if (isRight)
            rayOrigin.x -= (halfWidth - SkinWidth);
        // Analogiczna operacja, jesli poruszamy sie w lewo.
        else
            rayOrigin.x += (halfWidth - SkinWidth);

        // Ustalenie kierunku (lewo lub prawo), w jakim beda wysylane promienie.
        var rayDirection = isRight ? Vector2.right : -Vector2.right;
        // Ustawienie wartosci offsetu na zero.
        var offset = 0f;

        // Petle nie uwzgledniaja pierwszego i ostatniego promienia.
        for (var i = 1; i < TotalHorizontalRays - 1; i++)
        {
            // Ustalenie ktorego z poziomych promieni dotyczy 
            // biezaca iteracja petli, uwzgledniajac ruch gracza.
            // W przeciwnym wypadku podczas ruchu, wartosci te
            // bylyby opoznione o 1 klatke i promienie znajdowalyby sie poza graczem.
            var rayVector = new Vector2(deltaMovement.x + rayOrigin.x, deltaMovement.y + rayOrigin.y + (i * _verticalDistanceBetweenRays));
            // Debug.DrawRay(rayVector, rayDirection * halfWidth, isRight ? Color.cyan : Color.magenta);

            // PlatformMask informuje nas czy dany obiekt moze wstrzymac nasz ruch.
            // Tworzony jest wlasciwy promien docierajacy do celu.
            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, halfWidth, PlatformMask);
            // Sprawdzenie czy promien istnieje i trafil na przeszkode.
            if (!raycastHit)
                continue;

            // Obliczenie wartosci offsetu. Dla ruchu w prawo:
            // raycastHit.point.x - odleglosc od lewego brzegu boxu do punktu kolizji z obiektem
            // (punkt ten znajduje sie wewnatrz gracza - promienie sa wyslyane od srodka do brzegu boxu kolizji)
            // - odleglosc od lewego brzegu gracza do jego centrum - polowa rozmiaru boxu kolizji.
            // Otrzymana liczba ma zwykle wartosc ujemna i mowi o jaka wartosc mowi byc cofniety, aby
            // nie nastapila kolizja z platforma. Analogicznie dla ruchu w lewo.
            offset = isRight ? ((raycastHit.point.x - _transform.position.x) - halfWidth) : (halfWidth - (_transform.position.x - raycastHit.point.x));
        }

        // Dodanie do obecnej pozycji w poziomie wartosci offsetu.
        // Spowoduje to "odbicie" gracza w prawo lub w lewo, po
        // poziomej kolziji z platforma.
        deltaMovement.x += offset;
    }

    // Oblicza pozycje promieni wykrywajacych kolizje na podstawie obecnej pozycji gracza.
    // Punkty, z ktorych wychodza promienie zmieniaja sie w kazdej klatce animacji.
    private void CalculateRayOrigins()
    {
        // size - rozmiar boxu uczestniczącego w wykrywaniu kolizji, 
        // uwzględniający lokalną skalę rozmiaru obiektu gracza, podzielony na pol.
        var size = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.x), _boxCollider.size.y * Mathf.Abs(_localScale.y)) / 2;
        // center - polozenie centrum boxu, zazwyczaj wartosc rowna lub bliska zeru.
        var center = new Vector2(_boxCollider.center.x * _localScale.x, _boxCollider.center.y * _localScale.y);
        // Na osi x (dla _raycastTopLeft, dwa pozostałe - analogicznie): 
        // pozycja gracza
        // + odchylenie zwiazane z polozeniem centrum boxu wzgledem centrum gracza (zwykle wartosc pomijalna)
        // - polowa rozmiaru boxu kolizji
        // + szerokosc skin
        _raycastTopLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y + size.y - SkinWidth);
        _raycastBottomRight = _transform.position + new Vector3(center.x + size.x - SkinWidth, center.y - size.y + SkinWidth);
        _raycastBottomLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y - size.y + SkinWidth);
    }

    // deltaMovement - obecna pozycja gracza.
    // Ruch gracza w poziomie.
    // Ruch gracza w poziomie. Za pomocą promieni detekcji kolizji
    // ruch gracza jest ograniczany po napotkaniu przeszkody.
    private void MoveHorizontally(ref Vector2 deltaMovement)
    {
        // Sprawdzamy czy idziemy w prawo.
        var isGoingRight = deltaMovement.x > 0;
        // Dlugosc promienia: wykonywany ruch w poziomie + szerokosc skin.
        var rayDistance = Mathf.Abs(deltaMovement.x) + SkinWidth;
        // Zwrot (lewo, prawo) promienia, zależna od zwrotu gracza.
        var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
        // Ustalenie punktu startowego promieni, zalezne od zwrotu gracza.
        var rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;

        // Petla dla wszystkich promieni horyzontalnych.
        for (var i = 0; i < TotalHorizontalRays; i++)
        {
            // Ustalenie ktorego z poziomych promieni dotyczy 
            // biezaca iteracja petli.
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);

            // PlatformMask informuje nas czy dany obiekt moze wstrzymac nasz ruch.
            // Tworzony jest wlasciwy promien docierajacy do celu.
            var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
            // Sprawdzenie czy promien istnieje i trafil na przeszkode.
            if (!rayCastHit)
                continue;

            // Sprawdzenie czy wchodzimy na powierzchnie pochyla.
            if (i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(rayCastHit.normal, Vector2.up), isGoingRight))
                break;

            // Wlasciwe ograniczanie - jesli cos trafilismy, nie mozemy przejsc przez przeszkode.
            deltaMovement.x = rayCastHit.point.x - rayVector.x;
            rayDistance = Mathf.Abs(deltaMovement.x);

            // Zapobiegamy nachodzeniu tekstur, uwzgledniajac szerokosc skin dla ruchu do gory.
            if (isGoingRight)
            {
                deltaMovement.x -= SkinWidth;
                State.IsCollidingRight = true;
            }
            // Analogiczna operacja dla ruchu w dol.
            else
            {
                deltaMovement.x += SkinWidth;
                State.IsCollidingLeft = true;
            }

            // Obslugujemy sytuacje gdy nastapil niewielki blad w
            // obliczeniach ruchu, na skutek czego wychodzimy z petli.
            if (rayDistance < SkinWidth + .0001f)
                break;
        }
    }

    // deltaMovement - obecna pozycja gracza.
    // Ruch gracza w pionie.
    // Ograniczanie ruchu poprzez wykrycie kolizji w poziomie.
    // Zabezpiecza przed nachodzeniem gracza na podloze lub przeszkode nad nim.
    private void MoveVertically(ref Vector2 deltaMovement)
    {
        // Sprawdzamy czy porusza sie do gory.
        var isGoingUp = deltaMovement.y > 0;
        // // Dlugosc promienia: wykonywany ruch w pionie + szerokosc skin.
        var rayDistance = Mathf.Abs(deltaMovement.y) + SkinWidth;
        // Zwrot (gora, dol) promienia, zależna od zwrotu gracza.
        var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
        // Ustalenie punktu startowego promieni, zalezne od zwrotu gracza.
        // Zarowno dla rayOrigin w pionie i poziomie potrzebujemy
        // dwoch punktow, a jeden z nich moze byc uzyty w obu przypadkach,
        // co tlumaczy brak _raycastTopRight w programie.
        var rayOrigin = isGoingUp ? _raycastTopLeft : _raycastBottomLeft;

        // Ruch w poziomie zostal juz wykonany, wiec aktualizujemy 
        // pozycje startowa promeinia tak, aby
        // znajdowac sie w pozycji, do ktorej doszlimy, a nie tej
        // z ktorej zaczynalismy ruch.
        rayOrigin.x += deltaMovement.x;

        var standingOnDistance = float.MaxValue;
        // Petla dla wszystkich promieni wertykalnych.
        for (var i = 0; i < TotalVerticalRays; i++)
        {
            // Ustalenie ktorego z pionowych promieni dotyczy 
            // biezaca iteracja petli.
            var rayVector = new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);

            // PlatformMask informuje nas czy dany obiekt moze wstrzymac nasz ruch.
            // Tworzony jest wlasciwy promien docierajacy do celu.
            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
            // Sprawdzenie czy promien istnieje i trafil na przeszkode.
            if (!raycastHit)
                continue;

            // Jesli poruszamy sie w dol, a nasz promien wykryl przeszkode pod graczem.
            if (! isGoingUp)
            {
                // Odleglosc w pionie miedzy obecna pozycja gracza, a celem,
                // do ktorego zostal wystrzelony promien kolizji zwiazany z obecnym ruchem.
                var verticalDistanceToHit = _transform.position.y - raycastHit.point.y;

                // Aktualizacja wartosci mowiacej jaka odleglosc dzieli nas od 
                // zatrzymania sie i staniecia na platformie.
                if (verticalDistanceToHit < standingOnDistance)
                {
                    standingOnDistance = verticalDistanceToHit;
                    // StandingOn - obiekt (platforma), na ktorym stanie gracz. 
                    StandingOn = raycastHit.collider.gameObject;
                }
            }

            // Wlasciwe ograniczanie - jesli cos trafilismy, nie mozemy przejsc przez przeszkode.
            deltaMovement.y = raycastHit.point.y - rayVector.y;
            rayDistance = Mathf.Abs(deltaMovement.y);

            // Zapobiegamy nachodzeniu tekstur, uwzgledniajac szerokosc skin dla ruchu do gory.
            if (isGoingUp)
            {
                deltaMovement.y -= SkinWidth;
                State.IsCollidingAbove = true;
            }
            // Analogiczna operacja dla ruchu w dol.
            else
            {
                deltaMovement.y += SkinWidth;
                State.IsCollidingBelow = true;
            }

            // Jesli isGoingUp ma wartość false i nasza predkosc jest
            // wieksza od bardzo malej wartosci, oznacza to, że
            // poruszamy sie w gore po pochylej powierzchni.
            if (!isGoingUp && deltaMovement.y > .0001f)
                State.IsMovingUpSlope = true;

            // Obslugujemy sytuacje gdy nastapil niewielki blad w
            // obliczeniach ruchu, na skutek czego wychodzimy z petli.
            if (rayDistance < SkinWidth + .0001f)
                break;
            
        }
    }

    // Obsluga poruszania sie po pochylej powierzchni w pionie.
    private void HandleVerticalSlope(ref Vector2 deltaMovement)
    {
        // Pozycja centralna uzyskana na skutek dodania pozycji punktow startowych promieni
        // na obu koncach gracza i podzieleniu na pol.
        var center = (_raycastBottomLeft.x + _raycastBottomRight.x) / 2;
        // Zwrot ustawiony na "dol".
        var direction = -Vector2.up;

        var slopeDistance = SlopeLimitTangent * (_raycastBottomRight.x - center);
        // Wektor wskazujacy punkt znajdujacy sie w polozeniu centralnym na osi x,
        // oraz na wysokosci lewego dolnego rogu na osi y.
        var slopeRayVector = new Vector2(center, _raycastBottomLeft.y);
        Debug.DrawRay(slopeRayVector, direction * slopeDistance, Color.yellow);

        // PlatformMask informuje nas czy dany obiekt moze wstrzymac nasz ruch.
        // Tworzony jest wlasciwy promien docierajacy do celu.
        var raycastHit = Physics2D.Raycast(slopeRayVector, direction, slopeDistance, PlatformMask);
        // Sprawdzenie czy promien istnieje i trafil na przeszkode.
        if (!raycastHit)
            return;

        // Sign wykrywa znak wartosci, jesli wartosci maja taki sam znak,
        // oznacza to, ze poruszamy sie w dol po pochylej powierzchni.
        var isMovingDownSlope = Mathf.Sign(raycastHit.normal.x) == Mathf.Sign(deltaMovement.x);
        // Wychodzimy, jesli nie poruszamy sie w dol po pochylej powierzchni.
        if (!isMovingDownSlope)
            return;

        // Kat nachylenia miedzy wysylanym promieniem, a pionem.
        var angle = Vector2.Angle(raycastHit.normal, Vector2.up);
        // Wychodzimy, jesli kat ma wartosc bliska zeru.
        if (Mathf.Abs(angle) < .0001f)
            return;

        // Poruszamy sie w dol po pochylej powierzchni.
        State.IsMovingDownSlope = true;
        // Ustawiamy kat nachylenia.
        State.SlopeAngle = angle;
        // // Wlasciwe ograniczanie - jesli cos trafilismy, nie mozemy przejsc przez przeszkode.
        deltaMovement.y = raycastHit.point.y - slopeRayVector.y;
    }

    // Obsluga poruszania sie po pochylej powierzchni w poziomie.
    private bool HandleHorizontalSlope(ref Vector2 deltaMovement, float angle, bool isGoingRight)
    {
        // Sprawdzanie czy sprawdzana powierzchnia nie jest pionowa (jest sciana).
        if (Mathf.RoundToInt(angle) == 90)
            return false;

        // Jesli kat nachylenia jest wiekszy niz dopuszczalny,
        // ruch nie jest wykonywany.
        if (angle > Parameters.SlopeLimit)
        {
            deltaMovement.x = 0;
            return true;
        }

        // Jesli poruszamy sie do gory.
        if (deltaMovement.y > .07f)
            return true;

        // Do pozycji w poziomie dodawana jest szerokosc skin (ruch w lewo),
        // lub odejmowana (ruch w prawo).
        deltaMovement.x += isGoingRight ? -SkinWidth : SkinWidth;
        // Obliczana jest pozycja w pionie, na podstawie
        // kata nachylenia oraz pozycji w poziomie.
        deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);
        // Wystepuje kolizja z podlozem, oraz poruszamy sie w gore po pochylej powierzchni.
        State.IsMovingUpSlope = true;
        State.IsCollidingBelow = true;

        return true;
    }

    // Wykrycie ze gracz znalazl sie w innym srodowisku (np. wodzie).
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D>();

        if (parameters == null)
            return;

        // Nadpisanie parametrow kontrolera.
        _overrideParameters = parameters.Parameters;
    }

    // Wyjscie ze srodowiska o zmienionych parametrach.
    public void OnTriggerExit2D(Collider2D other)
    {
        var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D>();

        if (parameters == null)
            return;

        // Parametry nie sa juz nadpisywane poprzednimi wartosciami.
        _overrideParameters = null;
    }
}
