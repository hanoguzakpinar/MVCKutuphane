UPDATE Kitap
SET EmaneteUygunmu = 1

select * from Kitap

select * from EmanetKitaplar

select * from Kategori

alter table Kitap
alter column DeweyKod int





create view ViewEmanetKitaplar
AS

Select ek.ID, ek.KitapId, ek.KullaniciId, ek.AldigiTarih, ek.VerecegiTarih, ek.VerdigiTarih,
cast(case when ek.VerdigiTarih IS NULL then 0 else 1 end as bit) as TeslimDurumu,

cast(case when ek.VerdigiTarih is null 
then 
	cast(case when DATEDIFF(day,ek.VerecegiTarih, getdate()) > 30 
	then 
		(DATEDIFF(day,ek.VerecegiTarih, getdate())) * 0.25
	else 
		0 
	end as money)
else 
cast(case when DATEDIFF(day,ek.VerecegiTarih, ek.VerdigiTarih) > 30 
	then 
		(DATEDIFF(day,ek.VerecegiTarih, ek.VerdigiTarih)) * 0.25
	else 
		0 
	end as money) 
end as money) as Borcu,

ek.HasarDurumu, 
ek.HasarYeri, u.Adi + ' ' + u.Soyadi as kullaniciAdiSoyadi, k.Adi as kitapAdi

from EmanetKitaplar as ek
inner join Users as u on u.ID = ek.KullaniciId
inner join Kitap as k on k.Id = ek.KitapId

alter table Yazar
alter column Aciklama nvarchar(max)