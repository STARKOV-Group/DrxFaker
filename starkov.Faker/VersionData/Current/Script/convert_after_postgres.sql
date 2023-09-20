do $$
begin
  if exists(select 1 from information_schema.tables where table_schema = 'public' and table_name = 'starkov_faker_modulesetup') then 
    if exists(select 1 from information_schema.columns where table_name='starkov_faker_modulesetup' and column_name='isseparateasyn') then
      update starkov_faker_modulesetup set isseparateasyn = false where isseparateasyn is null;
    end if;
  end if;
end $$