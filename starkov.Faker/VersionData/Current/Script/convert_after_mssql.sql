if exists(select 1 from information_schema.tables where table_schema = 'dbo' and table_name = 'Starkov_Faker_ModuleSetup') 
  if exists(select 1 from information_schema.columns where table_name='Starkov_Faker_ModuleSetup' and column_name='IsSeparateAsyn')
    update Starkov_Faker_ModuleSetup set IsSeparateAsyn = 0 where IsSeparateAsyn is NULL;