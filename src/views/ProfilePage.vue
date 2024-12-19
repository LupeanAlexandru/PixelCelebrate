<template>
  <div>
    <h1>User Profile</h1>
    <div v-if="user">
      <p><strong>First Name:</strong> {{ user.firstName }}</p>
      <p><strong>Last Name:</strong> {{ user.lastName }}</p>
      <p><strong>Birthday:</strong> {{ formattedBirthday }}</p>
      <p><strong>Email:</strong> {{ user.email }}</p>
    </div>
    <div v-else>
      <p>Loading user data...</p>
    </div>
    <button @click="goHome">Home</button>
  </div>
</template>

<script>
export default {
  name: "ProfilePage",
  data() {
    return {
      user: null,
    };
  },
  created() {
    const storedUserData = localStorage.getItem("userData");
    if (storedUserData) {
      this.user = JSON.parse(storedUserData);
    } else {
      this.$router.push("/login");
    }
  },
  computed: {
    formattedBirthday() {
      if (this.user && this.user.birthDay) {
        const date = new Date(this.user.birthDay);
        const day = String(date.getDate()).padStart(2, "0");
        const month = String(date.getMonth() + 1).padStart(2, "0");
        const year = date.getFullYear();
        return `${day}/${month}/${year}`;
      }
      return "N/A";
    },
  },
  methods: {
    goHome() {
      this.$router.push({ name: "HomePage" });
    },
  },
};
</script>

<style scoped>
div {
  padding: 20px;
  font-family: Arial, sans-serif;
}

h1 {
  color: #333;
}

p {
  font-size: 16px;
  margin: 5px 0;
}

button {
  margin: 10px;
  padding: 10px 20px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

button:hover {
  background-color: #0056b3;
}
</style>
