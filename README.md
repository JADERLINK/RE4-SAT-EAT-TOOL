# RE4-SAT-EAT-TOOL
Extract and repack RE4 SAT-EAT files (RE4 2007/PS2/UHD/PS4/NS/GC/WII/XBOX360/RE4VR)

**Translate from Portuguese Brazil**

Programas destinados a extrair e reempacotar arquivo SAT e EAT do RE4;
<br>SAT: é o arquivo de colisão para o player, inimigos, Ashley e a câmera;
<br>EAT: é o arquivo de colisão para os projéteis, granadas e ovos, e para os itens que cai ao atirar neles;
<br>Nota: Fiz vários testes, porém não posso garantir que vai funcionar 100% das vezes, caso você encontre algum erro que possa ser do programa, me contate (e-mail);
<br>Aviso: as Normals das faces do modelo 3d para SAT/EAT devem ser do tipo "Flat", isto é, a normal tem que ser perpendicular à face (angulo de 90º), se não fizer isso a colisão vai ficar bugando.

**Update V.1.2.0**
<br>Adicionado suporte para as versões Big Endian (GC/WII/Xbox360);
<br>adicionado suporte para a versão Re4VR (o magic desse versão é 0xFF)

**Update B.1.1.1**
<br> Corrigido erro no repack no qual algumas faces nos limites do "groupTier0" ficavam fora dos grupos, e não funcionava a colisão.

**Update B.1.1.0**
<br>Adicionado suporte para as versões de PS4 e NS;

**Update B.1.0.2**
<br>Nessa nova versão, para arquivos "0000.SAT" e "0000.EAT", irá gerar arquivos .obj de nome "0000_SAT_0.obj" e "0000_EAT_0.obj" respectivamente, mudança feita para evitar sobreposição de arquivos.
<br>Agora, o programa, ao gerar o arquivo .obj, não terá mais os zeros não significativos dos números, mudança feita para gerar arquivos menores.

**Update B.1.0.0.1**
<br>Corrigida a compatibilidade com 3dsMax, agora o programa aceita nomes de grupos com o caractere Underline ( _ );

## RE4_SAT_EAT_EXTRACT.exe

Programa destinado a extrair o arquivo SAT ou EAT;
(escolha o .bat da mesma versão do seu jogo, caso você escolher a versão errada, o programa não vai dar erro, porém o arquivo .obj vai ficar errado) 
O programa irá gerar os arquivo:

* .OBJ, esse é o arquivo no qual vai ser editado, veja as informações abaixo.
* .IDXSAT, esse arquivo é necessário para o repack, com ele você irá recriar um arquivo SAT;
* .IDXEAT, esse arquivo é necessário para o repack, com ele você irá recriar um arquivo EAT;
* Nota: você pode receber de 1 a vários arquivos .obj;
* Nota2: você só vai ter somente um dos dois IDX vai depender de qual arquivo você extraiu;

**OBJ FILE**

A escala do arquivo é 100 vezes menor que a do jogo, sendo Y a altura.
<br> O nome desse arquivo é o nome do arquivo extraído mais um número, ex: filename_0.obj

Veja o exemplo:
<br>![exemplo](exemplo.png)
<br> O nome dos objetos tem que ser exatamente como é descrito abaixo:
<br> Nota: o programa não diferencia letras minúsculas de maiúsculas.

A nomenclatura dos nomes dos grupos/objetos pode ser:
<br>**Collision#00-00-00#**
<br>**Collision\_00\_00\_00\_**
<br>**Collision-00-00-00-**

Sendo:
* É obrigatório o nome do grupo começar com "Collision", e ser divido por # ou _
* A ordem dos campos não pode ser mudada;
* Sendo 00 um número em hexadecimal que vai de 00 a FF;
* Os números devem ser divididos pelo sinal de menos -
* Esses números, na verdade, são um conjunto de flags, então cada bit significa uma coisa, veja a sessão de **Flags** para saber mais;
* Cada um desses 3 bytes represento com uma cor, sendo na ordem: Blue, Green e Red;

 ----> Sobre verificações de grupos:
<br> * No Repack se ao lado direito do nome do grupo aparecer o texto "The group name is wrong;", significa que o nome do grupo está errado, e o seu arquivo vai ficar errado;
<br> * E se ao lado direito aparecer "Warning: Group not used;" esse grupo esta sendo ignorado pelo meu programa, caso, na verdade, você gostaria de usá-lo, você deve arrumar o nome do grupo;


## RE4_SAT_EAT_REPACK.exe

Programa destinado a reempacotar o arquivo SAT-EAT;
<br> Nota: escolha o .bat da mesma versão do seu jogo.
<br> (Mas caso você queira converter o arquivo de uma versão para outra, é só escolher a versão do jogo na qual você vai colocar o seu arquivo.)


## Sobre .idxsat / .idxeat
Os dois tipos de idx têm o mesmo tipo de conteúdo, o nome é diferente para que o arquivo gerado pelo repack seja do formato correto;
<br> Nota: Na verdade, você não precisa editar o conteúdo desse arquivo;
<br> Segue abaixo a lista de comando presente no arquivo:
* Magic: o valor pode ser 80 ou 20, sendo que 80 pode ter 1 ou mais arquivo .obj, e 20 somente 1 arquivo .obj;
* Count: essa é a quantidade de arquivos obj;
* Dummy: esse é um campo em hexadecimal de 2 bytes, esse campo só é valido se o Magic for 80, não sei para que serve esse campo no arquivo, mas é para não ter utilidade no arquivo.

## Sobre Flags e converter da "SoP Tool"
Abaixo vou explicar como converter o seu arquivo usado na Tool do SoP-SAT-EAT, para a minha Tool;
<br> Aviso: faça backup de seus arquivos antes de mudar, não me responsabilizo por perdas de dados;
<br> Para saber os nomes das flags de cada byte veja o arquivo [Flags.md](https://github.com/JADERLINK/RE4-SAT-EAT-TOOL/blob/main/Flags.md) desse repositório;
<br> Atenção: As flags usadas no Eat são diferentes das usadas no Sat, então mesmo que sejam o mesmo valor, os significados são diferentes;
<br> Para cada byte, eu nomeei de uma cor:

Minha tool para ambas as versões do jogo:
<br>**Collision#BB-GG-RR#**
<br>
<br>Para a tool do "Son of Percia" a ordem dos bytes muda de acordo com a versão do jogo:
<br>
<br> Para UHD:
<br>**object\_?\_0xNN\_0xNN\_0xRR\_0x0xYY\_0xBB\_0xGG**
<br>
<br> Para 2007-PS2:
<br>**object\_?\_0xNN\_0xNN\_0xBB\_0xGG\_0xRR\_0xYY**
<br>
<br> sendo:
* **NN**: o valor desses dois bytes é sempre 0 (zero), eles não são utilizados pelo jogo;
* **BB**: esse é o byte de cor BLUE;
* **GG**: esse é o byte de cor GREEN;
* **RR**: esse é o byte de cor RED;
* **YY**: esse é o byte de cor YELLOW, esse byte é omitido no meu programa, pois o programa calcula o valor desse byte automaticamente, ele define as conexões das edges de cada triangulo/face;

## Código de terceiro:

[ObjLoader by chrisjansson](https://github.com/chrisjansson/ObjLoader):
Encontra-se em RE4_SAT_EAT_TOOL/CjClutter.ObjLoader.Loader, código modificado, as modificações podem ser vistas aqui: [link](https://github.com/JADERLINK/ObjLoader).

**At.te: JADERLINK**
<br>Thanks to "mariokart64n" and "zatarita"
<br>2024-11-25