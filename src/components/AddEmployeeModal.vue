<template>
  <div v-if="visible" class="modal-overlay">
    <div class="modal">
      <h2>Add Employee</h2>
      <form @submit.prevent="submit">
        <div>
          <label>First Name:</label>
          <input v-model="firstName" type="text" required />
        </div>
        <div>
          <label>Last Name:</label>
          <input v-model="lastName" type="text" required />
        </div>
        <div>
          <label>Email:</label>
          <input v-model="email" type="email" required />
        </div>
        <div>
          <label>Password:</label>
          <input v-model="password" type="password" required />
        </div>
        <div>
          <label>Birthday:</label>
          <input v-model="birthDay" type="date" required />
          <span v-if="birthdayError" style="color: red"
            >Birthday cannot be in the future.</span
          >
        </div>
        <div>
          <label>Admin:</label>
          <input v-model="isAdmin" type="checkbox" />
        </div>
        <div>
          <button type="submit">Submit</button>
          <button type="button" @click="close">Cancel</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    visible: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      birthDay: "",
      isAdmin: false,
    };
  },
  computed: {
    birthdayError() {
      const selectedDate = new Date(this.birthDay);
      const today = new Date();
      today.setHours(0, 0, 0, 0);
      return selectedDate > today;
    },
  },
  methods: {
    submit() {
      if(this.birthdayError){
        return;
      }
      const newUser = {
        firstName: this.firstName,
        lastName: this.lastName,
        email: this.email,
        password: this.password,
        birthDay: this.birthDay,
        isAdmin: this.isAdmin,
      };
      this.$emit("submit", newUser);
      this.resetForm();
    },
    close() {
      this.$emit("close");
      this.resetForm();
    },
    resetForm() {
      this.firstName = "";
      this.lastName = "";
      this.email = "";
      this.password = "";
      this.birthDay = "";
      this.isAdmin = false;
    },
  },
};
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
}

.modal {
  background: white;
  padding: 20px;
  border-radius: 8px;
  width: 400px;
}

.modal h2 {
  margin-top: 0;
}

.modal form div {
  margin-bottom: 15px;
}

.modal form label {
  display: block;
  font-weight: bold;
  margin-bottom: 5px;
}

.modal form input {
  width: 100%;
  padding: 8px;
  box-sizing: border-box;
}

.modal form button {
  margin-right: 10px;
  padding: 10px 20px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.modal form button:hover {
  background-color: #0056b3;
}

.modal form button[type="button"] {
  background-color: #ccc;
}

.modal form button[type="button"]:hover {
  background-color: #999;
}
</style>
