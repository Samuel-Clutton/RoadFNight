using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Shooting : NetworkBehaviour
{
	
	// PSEUDO 
		// this all needs editing as it is rubbish
	
	
	private Transform _bulletSpawnPointPosition;
	private Transform _cartridgeEjectSpawnPointPosition;

	private GameObject _cartridgeEjectPrefab;


	private PlayerInventoryModule _pIm;
	private Player _player;
	
	private void Start()
	{
		_pIm = GetComponent<PlayerInventoryModule>();
		_player = GetComponent<Player>();
	}


	private void Update()
	{
		AimWeapon();
	}

	public void ChangeWeapon()
	{
		
	}
	

	private bool CheckCanAim()
	{
		if (_pIm == null)
		{
			Debug.LogError("_pIm is null");
			return false;
		}

		if (_pIm.inputs == null)
		{
			Debug.LogError("_pIm.inputs is null");
			return false;
		}

		if (_pIm.health == null)
		{
			Debug.LogError("_pIm.health is null");
			return false;
		}

		if (this.GetComponent<EmoteWheel>() == null)
		{
			Debug.LogError("EmoteWheel component is missing");
			return false;
		}

		if (_pIm.slot.item.itemSO == null)
		{
			Debug.LogError("_pIm.slot.item.itemSO is null");
			return false;
		}
		
		return _pIm.inputs.aim &
			   !_pIm.inShop & 
		       !_pIm.inCar & 
		       !_pIm.usesParachute &
		       !_pIm.health.isDeath &
		       !this.GetComponent<EmoteWheel>().isPlayingAnimation & 
		       _pIm.slot.amount > 0 &
		       _pIm.slot.item.itemSO != null & 
		       _pIm.slot.item.itemSO is WeaponItemSO;
	}
	
	public void AimWeapon()
	{
		//Aim
		if (CheckCanAim())
		{
			CmdAim();
		}
	}

	
	/// <summary>
	/// This will push to the server that the player is aiming to the TP controller setting the aim value to 1
	/// this likely is working in parallel to setting the animator values for the server?
	/// </summary>
    [Command]
    public void CmdAim()
    {
	    _pIm.TPControllerManager.aimValue = 1;
    }

	/// <summary>
	/// Spawns a bullet at the location of the current weapon spawn then ejects bullet towards a location using the screen point to ray
	/// method from unity
	/// Uses an if Statement for that of rockets or bullets
	///
	///
	/// These need changing need to look for swap weapon, then call the change of current weapon bullet spawn to be only called on weapon change and awake (if has a weapon)
	/// or maybe add a debounce to have a more simple approach.
	/// Switch statement to replace the if statement so we can include different bullet types such as 9mm etc.
	/// </summary>
    public void ShootBullet()
    {
        if (!base.hasAuthority) return;

        _bulletSpawnPointPosition = this.GetComponent<ManageTPController>().CurrentWeaponBulletSpawnPoint;
        _cartridgeEjectSpawnPointPosition = this.GetComponent<ManageTPController>().CurrentCartridgeEjectSpawnPoint;
        string currentBulletName = this.GetComponent<ManageTPController>().CurrentWeaponManager.WeaponBulletPrefab.name;
        //bulletSpeed = this.GetComponent<ManageTPController>().CurrentWeaponManager.BulletSpeed;
        this.GetComponent<ManageTPController>().Shoot();
        //CmdSetAttackerUsername(username);

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        int layerToIgnore = 4; // Replace with the layer you want to ignore
        LayerMask layerMask = ~(1 << layerToIgnore);
        RaycastHit hit;
        Vector3 collisionPoint;
        if (Physics.Raycast(ray, out hit, 50f, layerMask))
        {
            collisionPoint = hit.point;
        }
        else
        {
            collisionPoint = ray.GetPoint(50f);
        }

        Vector3 bulletVector = (collisionPoint - _bulletSpawnPointPosition.transform.position).normalized;




        if (currentBulletName == "Bullet")
            CmdShootBullet(_bulletSpawnPointPosition.position, _bulletSpawnPointPosition.rotation, _cartridgeEjectSpawnPointPosition.position, _cartridgeEjectSpawnPointPosition.rotation, bulletVector, _pIm.bulletSpeed);
        else
            CmdShootRocket(_bulletSpawnPointPosition.position, _bulletSpawnPointPosition.rotation, bulletVector, _pIm.bulletSpeed);
    }

	/// <summary>
	/// Server command to shoot the bullet after the shoot bullet client side has been called, calculating which type of projectile will be fired.
	/// Then it will use the position of the pawn rotation, position of the cartridge ejection, spawn point position of the cartridge, bullet direction of shooting and its speed.
	/// This will then move to a remote procedure to fire this event from the client to the network. 
	/// </summary>
	/// <param name="_position"></param>
	/// <param name="_rotation"></param>
	/// <param name="_cartridgeEjectPosition"></param>
	/// <param name="_cartridgeEjectRotation"></param>
	/// <param name="_bulletVector"></param>
	/// <param name="_bulletSpeed"></param>
    [Command]
    void CmdShootBullet(Vector3 _position, Quaternion _rotation, Vector3 _cartridgeEjectPosition, Quaternion _cartridgeEjectRotation, Vector3 _bulletVector, float _bulletSpeed)
    {
        GameObject Bullet = Instantiate(_pIm.bulletPrefab, _position, _rotation) as GameObject;

        Bullet.GetComponent<Rigidbody>().velocity = _bulletVector * _bulletSpeed;

        //Bullet.GetComponent<NetworkBullet>().SetupProjectile(this.GetComponent<Player>().username, hasAuthority);

        NetworkServer.Spawn(Bullet);

        NetworkBullet bullet = Bullet.GetComponent<NetworkBullet>();
        bullet.netIdentity.AssignClientAuthority(this.connectionToClient);

        bullet.SetupProjectile_ServerSide();

        RpcBulletFired(bullet, _bulletVector, _bulletSpeed);


        GameObject _cartridgeEject = Instantiate(_cartridgeEjectPrefab, _cartridgeEjectPosition, _cartridgeEjectRotation) as GameObject;



        NetworkServer.Spawn(_cartridgeEject, connectionToClient);
    }

	/// <summary>
	/// Sets up the projectile for who fired it, and then also set the autority to the player who fired it,
	/// this seems to be to prevent the player from getting hit by their own bullets.
	/// </summary>
	/// <param name="Bullet"></param>
	/// <param name="_bulletVector"></param>
	/// <param name="_bulletSpeed"></param>
    [ClientRpc]
    void RpcBulletFired(NetworkBullet Bullet, Vector3 _bulletVector, float _bulletSpeed)
    {
        Bullet.GetComponent<NetworkBullet>().SetupProjectile(_player.username, hasAuthority);

        //Bullet.GetComponent<Rigidbody>().AddForce(_bulletVector * _bulletSpeed);
    }

	/// <summary>
	/// Commands the server to instantiate a rocket prefab at the position of the weapon spawn. this is passed from the shoot bullet method
	/// which allows us to carry over the positions of the spawn, its rotation, the direction it is going and the speed.
	///
	/// Naming convention on this and a few others is pretty bad so that needs editing
	/// </summary>
	/// <param name="_position"></param>
	/// <param name="_rotation"></param>
	/// <param name="_bulletVector"></param>
	/// <param name="_bulletSpeed"></param>
    [Command]
    void CmdShootRocket(Vector3 _position, Quaternion _rotation, Vector3 _bulletVector, float _bulletSpeed)
    {
        GameObject Bullet = Instantiate(_pIm.rocketPrefab, _position, _rotation) as GameObject;

        //Bullet.GetComponent<Rigidbody>().AddForce(_bulletVector * _bulletSpeed);

        //Bullet.GetComponent<NetworkBullet>().SetupProjectile(this.GetComponent<Player>().username, hasAuthority);

        NetworkServer.Spawn(Bullet);

        NetworkRocket bullet = Bullet.GetComponent<NetworkRocket>();
        bullet.netIdentity.AssignClientAuthority(this.connectionToClient);

        bullet.SetupProjectile_ServerSide();

        RpcRocketFired(bullet, _bulletVector, _bulletSpeed);
    }

	/// <summary>
	/// Remote to send the projectile setup from the client to the server, using the method aboves
	/// positional data and direction 
	/// </summary>
	/// <param name="Bullet"></param>
	/// <param name="_bulletVector"></param>
	/// <param name="_bulletSpeed"></param>
    [ClientRpc]
    void RpcRocketFired(NetworkRocket Bullet, Vector3 _bulletVector, float _bulletSpeed)
    {
        Bullet.GetComponent<NetworkRocket>().SetupProjectile(_player.username, hasAuthority);

        Bullet.GetComponent<Rigidbody>().AddForce(_bulletVector * _bulletSpeed);
    }
}
