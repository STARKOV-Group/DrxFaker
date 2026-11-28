if exists(select 1 from information_schema.tables where table_schema = 'dbo' and table_name = 'Starkov_Faker_ModuleSetup') 
  if exists(select 1 from information_schema.columns where table_name='Starkov_Faker_ModuleSetup' and column_name='IsSeparateAsyn')
    update Starkov_Faker_ModuleSetup set IsSeparateAsyn = 0 where IsSeparateAsyn is NULL;
    
if exists(select 1 from information_schema.tables where table_schema = 'dbo' and table_name = 'Starkov_Faker_ModuleSetup') 
  if exists(select 1 from information_schema.columns where table_name='Starkov_Faker_ModuleSetup' and column_name='IsDisableNotif')
    update Starkov_Faker_ModuleSetup set IsDisableNotif = 0 where IsDisableNotif is NULL;
    
if exists(select 1 from information_schema.tables where table_schema = 'dbo' and table_name = 'Starkov_Faker_ParametersMatc') 
  if exists(select 1 from information_schema.columns where table_name='Starkov_Faker_ParametersMatc' and column_name='IsNeedStartTas')
    update Starkov_Faker_ParametersMatc set IsNeedStartTas = 0 where IsNeedStartTas is NULL;