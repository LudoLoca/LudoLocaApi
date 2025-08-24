# LudoLocaApi
[🔗 README e documentação Principais ](https://github.com/filipechgs/LudoLoca)

# Guia de Configuração e Versionamento - LudoLoca

---

## 1 Clonagem: Quando Usar o Meta Repo?
<details>
<summary>Ver instruções completas de clonagem</summary>

### Situação 1: Você só vai trabalhar na API ou no Client
Se você vai trabalhar **apenas** na API ou **apenas** no Client, **clone somente o repositório correspondente**.

-> Busque Item 12 Como Trabalhar Individualmente com API ou Client (Sem Meta Repo), ao final deste README

### Situação 2: Você vai rodar API e Client juntos, sincronizados no branch `develop`
Nesse caso, **clonar apenas um repositório não basta**. Você precisa do **meta-repo**, pois ele contém ambos (API e Client) configurados como submódulos.

- Para clonar o meta-repo já trazendo os submódulos:
```bash
git clone --recurse-submodules https://github.com/LudoLoca/LudoLocaMeta.git
```
```bash
cd LudoLocaMeta
```

- Se você esqueceu de usar `--recurse-submodules` na clonagem:
```bash
git submodule update --init --recursive
```

> [!NOTE]
> O meta-repo não contém código diretamente. Ele apenas referencia os repositórios **API** e **Client** como submódulos.
</details>

---

## 2 Como Configurar a Connection String com User Secrets
<details>
<summary>Ver instruções completas de configuração do User Secrets</summary>

O comando `dotnet user-secrets` permite armazenar informações sensíveis (como connection strings) **fora do código-fonte** e apenas no seu ambiente local de desenvolvimento.  
Isso garante que senhas e credenciais não sejam expostas no repositório Git.

### Quando rodar
Sempre que você precisar:
- Configurar a string de conexão do banco de dados localmente pela primeira vez
- Atualizar usuário ou senha do banco
- Trocar de banco de dados

### Onde rodar
Sempre execute dentro da pasta da **API**, onde está o arquivo `.csproj`.

### Passo a passo para definir a connection string

1. Abra o terminal e navegue até a pasta da API:
```bash
cd API
```

2. Execute o comando abaixo.  
   - Substitua **[SEU_USUARIO]** pelo usuário do banco de dados que você recebeu.  
   - Substitua **[SUA_SENHA]** pela senha atribuída ao seu usuário.  

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=shortline.proxy.rlwy.net;Port=48567;Database=ludolocadev;Username=[SEU_USUARIO];Password=[SUA_SENHA];SSL Mode=Require;Trust Server Certificate=true"
```

> [!IMPORTANT]  
> Não remova as aspas do comando. Substitua apenas os valores entre colchetes.

### Como verificar se funcionou
Liste todos os secrets configurados:
```bash
dotnet user-secrets list
```

Você deve ver a chave `ConnectionStrings:DefaultConnection` e o valor completo configurado.

Exemplo de saída esperada:
```
ConnectionStrings:DefaultConnection = Host=shortline.proxy.rlwy.net;Port=48567;Database=ludolocadev;Username=meuUsuario;Password=minhaSenha;SSL Mode=Require;Trust Server Certificate=true
```

### Como remover ou redefinir o secret
- Para remover a connection string atual:
```bash
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"
```

- Para redefinir com novos valores (exemplo de novo usuário/senha):
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=shortline.proxy.rlwy.net;Port=48567;Database=ludolocadev;Username=[NOVO_USUARIO];Password=[NOVA_SENHA];SSL Mode=Require;Trust Server Certificate=true"
```

### Onde os secrets são salvos
No Windows, os secrets ficam armazenados em:
```
%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json
```

- O valor de `<UserSecretsId>` está dentro do arquivo `.csproj` da API, dentro da tag `<UserSecretsId>...</UserSecretsId>`.

Para abrir o arquivo no Notepad:
```bash
notepad %APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json
```

> [!WARNING]  
> Nunca coloque a string de conexão diretamente no código ou no repositório.  
> O `user-secrets` existe exatamente para evitar esse risco.
</details>

---

## 3 Como Clonar o Meta Repo com Submódulos
<details>
<summary>Ver instruções detalhadas sobre submódulos</summary>

Para clonar o meta-repo junto com seus submódulos (API e Client):
```bash
git clone --recurse-submodules https://github.com/LudoLoca/LudoLocaMeta.git
cd LudoLocaMeta
```

Se você esqueceu de usar `--recurse-submodules` e já clonou o repositório:
```bash
git submodule update --init --recursive
```

> [!NOTE]  
> O comando acima garante que tanto o submódulo **API** quanto o submódulo **Client** sejam inicializados e sincronizados.
</details>

---

## 4 Como Funcionam os Submódulos e o Branch develop
<details>
<summary>Ver instruções detalhadas sobre submódulos e branch develop</summary>

O arquivo `.gitmodules` do meta-repo já está configurado para que **API** e **Client** apontem para o branch `develop`:

```ini
[submodule "Client"]
    path = Client
    url = https://github.com/LudoLoca/LudoLocaClient.git
    branch = develop

[submodule "API"]
    path = API
    url = https://github.com/LudoLoca/LudoLocaApi.git
    branch = develop
```

### Ponto importante
Quando você clona o meta-repo:
- Os submódulos **não** são checados automaticamente no último commit do `develop`
- Eles são checados no **commit exato** referenciado pelo meta-repo

### Como garantir que está no último commit do `develop`
Execute:
```bash
git submodule update --remote
```

Isso fará com que:
- O submódulo API avance para o último commit do branch `develop` no repositório da API
- O submódulo Client avance para o último commit do branch `develop` no repositório do Client

---

### O que acontece se alguém trocar o branch de um submódulo?
- Se você executar, por exemplo:
```bash
cd API
git checkout feature-x
```
Isso **não altera** o arquivo `.gitmodules`.  
O que muda é apenas o **ponteiro** do commit que o meta-repo está registrando.

Se você depois fizer:
```bash
git add API
git commit -m "Atualiza ponteiro do submódulo API para feature-x"
git push
```
O próximo desenvolvedor que clonar o meta-repo vai trazer exatamente esse commit da API, e não necessariamente o último do branch `develop`.

> [!CAUTION]  
> Alterar branch dentro de um submódulo **não altera** o branch padrão definido em `.gitmodules`.  
> O meta-repo apenas passa a apontar para o commit que você selecionou.
</details>

---

## 5 Fluxo de Trabalho
<details>
<summary>Ver instruções detalhadas de fluxo de trabalho</summary>

### Sempre usar o último commit do branch `develop` nos submódulos
Após clonar o meta-repo, rode:
```bash
git submodule update --remote
```

Esse comando força os submódulos **API** e **Client** a avançarem para o último commit do branch `develop` em seus respectivos repositórios.

---

### Travar o meta-repo em um commit específico de um submódulo
1. Entre na pasta do submódulo desejado (API ou Client).
```bash
cd API
```
ou
```bash
cd Client
```

2. Faça checkout do commit ou branch específico que deseja usar:
```bash
git checkout <commit-ou-branch>
```

3. Volte para a raiz do meta-repo e atualize o ponteiro do submódulo:
```bash
cd ..
git add API Client
git commit -m "Atualiza ponteiro dos submódulos"
git push
```

Resultado esperado: o meta-repo agora referencia exatamente o commit selecionado do submódulo.

---

### Mudando o branch de um submódulo no meta-repo
1. Entre na pasta do submódulo (API ou Client):
```bash
cd API
```

2. Troque para o branch desejado:
```bash
git checkout nome-do-branch
```

3. Envie a mudança do submódulo:
```bash
git push
```

4. Volte para o meta-repo e atualize o ponteiro:
```bash
cd ..
git add API
git commit -m "Atualiza submódulo API para branch nome-do-branch"
git push
```

> [!WARNING]  
> Isso **não altera o branch padrão** configurado em `.gitmodules`.  
> Apenas atualiza o meta-repo para apontar para o commit/branch que você selecionou.
</details>

---

## 6 Dicas Importantes
<details>
<summary>Ver boas práticas ao lidar com submódulos</summary>

- Nunca rode `git checkout` ou `git pull` em submódulos sem antes salvar ou commitar suas alterações locais.
- O meta-repo **não sobrescreve** os repositórios remotos de API ou Client. Ele apenas referencia commits específicos.
- Para garantir que você sempre esteja no último commit do `develop`, rode:
```bash
git submodule update --remote
```
- Se você mudar o branch de um submódulo e commitar no meta-repo, o próximo clone do meta-repo trará exatamente aquele commit específico, e não o último do `develop`.

> [!CAUTION]  
> Erros comuns acontecem quando alguém muda branch em um submódulo e esquece de atualizar corretamente o meta-repo. Sempre alinhe com o time.
</details>

---

## 7 Rodando no Visual Studio
<details>
<summary>Ver instruções completas para executar no Visual Studio</summary>

1. Abra o arquivo de solução `LudoLoca.sln`, localizado na raiz do meta-repo.
2. Configure múltiplos projetos de inicialização:
   - Clique com o botão direito na Solution
   - Selecione **Configure Startup Projects**
   - Escolha **Multiple startup projects**
   - Configure os projetos **API** e **Client** para a ação **Start**
3. Sempre que fizer alterações no código:
   - Use **Build > Clean Solution** e depois **Build > Rebuild Solution**
   - Alternativamente, clique com o botão direito em cada projeto (**API** ou **Client**) → **Clean** e depois → **Rebuild**
4. Para rodar:
   - Pressione `F5`
   - O Visual Studio abrirá o navegador com API e Client rodando em paralelo
5. Para parar:
   - Feche todas as abas abertas do navegador
   - Feche todas as janelas de CMD abertas automaticamente
   - Clique no botão quadrado vermelho no topo do Visual Studio

> [!TIP]  
> Usar **Clean/Rebuild** evita erros de build causados por cache.
</details>

---

## 8 Restaurar Pacotes NuGet
<details>
<summary>Ver instruções de restauração de pacotes</summary>

Normalmente, o Visual Studio restaura automaticamente todos os pacotes NuGet necessários.  

Se houver problemas de dependência, execute no console do Visual Studio:
```powershell
Update-Package -reinstall
```

> [!NOTE]  
> Esse comando força a reinstalação de todos os pacotes. Use apenas se o Visual Studio não conseguir resolver automaticamente.
</details>

---

## 9 Aplicar Migrations
<details>
<summary>Ver instruções para aplicar migrations</summary>

Somente necessário se houver alterações nas tabelas do banco de dados.  

1. Entre na pasta da API:
```bash
cd API
```

2. Execute a atualização do banco de dados:
```bash
dotnet ef database update
```

> [!WARNING]  
> Execute migrations apenas quando necessário e sempre confirme com o time.  
> Alterações não sincronizadas podem quebrar o banco de dados local de outros desenvolvedores.
</details>

---

## 10 Confiar no Certificado HTTPS
<details>
<summary>Ver instruções para confiar no certificado HTTPS</summary>

Se ao rodar o projeto o navegador não confiar na API ou no Client (e não abrir o site):
```bash
dotnet dev-certs https --trust
```

> [!NOTE]  
> Esse comando é necessário apenas se o navegador bloquear o acesso ao projeto local.
</details>

---

## 11 Rodar a Solução
<details>
<summary>Ver instruções resumidas para rodar a solução</summary>

- Abra o `LudoLoca.sln` no Visual Studio
- Configure múltiplos projetos de inicialização (API e Client)
- Pressione `F5` para rodar ambos
</details>

---

## 12 Como Trabalhar Individualmente com API ou Client (Sem Meta Repo)
<details>
<summary>Ver instruções completas para trabalhar sem meta-repo</summary>

Você pode trabalhar diretamente nos repositórios da API ou do Client sem usar o meta-repo.  
Cada repositório é independente e pode ser clonado, versionado e executado separadamente.

### Clonando a API
```bash
git clone --branch develop https://github.com/LudoLoca/LudoLocaApi.git
cd API
```

### Clonando o Client
```bash
git clone --branch develop https://github.com/LudoLoca/LudoLocaClient.git
cd Client
```

### Criando uma branch para sua feature
Nunca trabalhe diretamente no branch `develop`.  
Crie sempre uma branch antes de alterar qualquer código:
```bash
git checkout -b feat/nome-da-feature
```

### Versionando alterações
1. Verifique o status dos arquivos:
```bash
git status
```
2. Adicione arquivos alterados:
```bash
git add .
```
3. Faça commit:
```bash
git commit -m "feat: implementa nova funcionalidade X"
```
4. Envie sua branch para o repositório remoto:
```bash
git push origin feat/nome-da-feature
```

### Finalizando sua feature
- Verifique se sua branch compila e funciona corretamente
- Faça merge no branch `develop`
- O merge pode ser feito via Pull Request no GitHub ou manualmente, de acordo com o fluxo do time
</details>

---

## 13 Diferenças em relação ao Meta Repo
<details>
<summary>Ver diferenças entre meta-repo e repositórios individuais</summary>

- **No meta-repo:**  
  Você trabalha com os submódulos (API e Client) juntos e sincronizados.  
  É necessário atualizar o ponteiro do submódulo no meta-repo para refletir mudanças.

- **No repositório individual:**  
  Você trabalha, versiona e executa o projeto isoladamente.  
  Alterações feitas em um repositório individual **não afetam** o meta-repo, e vice-versa.
</details>

---

## Dicas de Segurança
<details>
<summary>Ver boas práticas de segurança</summary>

- Sempre use `git status` antes de qualquer comando, para garantir que você está no repositório e branch corretos.
- Nunca faça `git pull`, `git checkout` ou `git merge` sem antes commitar ou salvar seu trabalho local.
- Se não tiver certeza do impacto de um comando, faça backup dos arquivos antes de executar.

> [!CAUTION]  
> Grande parte dos erros em Git ocorrem por executar comandos no repositório ou branch errado.  
> **Sempre valide o contexto atual com `git status`.**
</details>
