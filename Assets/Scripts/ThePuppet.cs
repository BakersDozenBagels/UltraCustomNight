using System.Collections;
using UnityEngine;
using Rng = UnityEngine.Random;

class ThePuppet : Animatronic
{
    private ThePuppetScript _script;
    private ThePuppetColliderScript _TPCS;
    private bool _mousedown = true;

    public ThePuppet(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("The Puppet is coming to attack! Watch out for your mouse.");
        Instance.AddCoroutineNow(Move());

        GameObject b = instance.PublicInstantiate(instance.ThePuppetPrefab);
        _script = b.GetComponent<ThePuppetScript>();
        _script.SpriteRenderer.enabled = false;

        _TPCS = _script.GetComponentInChildren<ThePuppetColliderScript>();
        _TPCS.OMO += d => _mousedown = d;

        Instance.Destroy += () => { Destroy(); };
    }

    private void Destroy()
    {
        Object.Destroy(_script.gameObject);
    }

    private IEnumerator Move()
    {
        yield return WaitFor(Rng.Range(20f, 40f));
        Instance.Log("The puppet is attacking!");
        Vector3 pt = Input.mousePosition;
        pt.z = .1f;
        pt = Camera.main.ScreenToWorldPoint(pt);
        _script.SpriteTransform.position = pt;
        _script.SpriteTransform.localEulerAngles = new Vector3(0f, 0f, Rng.Range(0f, 360f));
        _script.SpriteRenderer.enabled = true;

        float opac = 0.01f;
        while(opac < 0.5f)
        {
            yield return null;
            opac += Time.deltaTime / 3f;
            _script.SpriteRenderer.material.SetFloat("_Opacity", opac);
        }
        while(opac > 0f)
        {
            yield return null;
            if(_mousedown)
            {
                opac += Time.deltaTime / 3f;
                if(opac >= 1f)
                {
                    Instance.Log("Strike from The Puppet!");
                    Instance.Strike();
                    break;
                }
            }
            else
                opac -= Time.deltaTime / 2f;
            _script.SpriteRenderer.material.SetFloat("_Opacity", opac);
        }
        _script.SpriteRenderer.material.SetFloat("_Opacity", 0f);

        Instance.AddCoroutineNow(Move());
    }
}
