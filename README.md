# LCSIQRCodePIX
DLL para geração de QRCode PIX para Boletos Híbridos

🧾 LCSIQRCodePIX
Este projeto em C# gera QR Codes no padrão EMV QR Code - PIX (BR Code), conforme especificações do Banco Central do Brasil. A biblioteca foi desenvolvida com suporte a COM Interop, permitindo integração com sistemas legados, como aplicações em VB6.

🚀 Funcionalidades
Geração do payload EMV QR Code para pagamentos via PIX

Criação de imagem PNG do QR Code a partir do payload gerado

Validação dos campos obrigatórios

Suporte a Point of Initiation Method (11 ou 12)

Integração com outras linguagens via interface COM visível

🧰 Tecnologias utilizadas
C#

ZXing.Net para geração do QR Code

System.Drawing para manipulação de imagens

COM Interop com atributos ComVisible, Guid e InterfaceType

📦 Métodos principais
GeneratePixPayload(...)
Gera o payload compatível com o padrão PIX a partir dos dados fornecidos (chave, valor, recebedor, cidade, TXID, etc.)

GenerateQRCode(...)
Gera e salva a imagem do QR Code em formato .png com base no payload do PIX

📂 Estrutura COM
A biblioteca está preparada para ser registrada como um COM Object, podendo ser chamada por sistemas em VB6, Excel VBA, ou outras linguagens que suportem objetos COM.
