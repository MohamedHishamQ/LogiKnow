

async function test() {
  const payload = {
    messages: [{ role: "user", content: "Hi" }],
    locale: "en"
  };
  const res = await fetch('http://localhost:3000/api/chat', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  console.log('Next.js API status:', res.status);
  console.log(await res.text());
}
test();
